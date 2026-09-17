
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
    [Table("ModelAnnotations")]
    public class ModelAnnotationBase : SoAEntityBase
    {
        [Key]
        public string ModelAnnotationId { get; set; }

        // Formula Name (rulebook: ={{AnnotationKind}} & ": " & LEFT({{Body}}, 50))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.AnnotationKind)), F.S(": "), F.Text(F.Left(F.Of(this.Body), F.I(50)))))); set { }
        }

        public DateTimeOffset? AnnotatedAt { get; set; }
        public string? AnnotationKind { get; set; }
        public string? Body { get; set; }
        public string? Status { get; set; }
        public string? EnteredThrough { get; set; }
        public string? StoredIn { get; set; }
        // Formula IsDiscussionInAuthoritativeModel (rulebook: =AND(OR({{AnnotationKind}} = "Comment", {{AnnotationKind}} = "Question", {{AnnotationKind}} = "Suggestion", {{AnnotationKind}} = "AlternativeInterpretation"), {{StoredIn}} = "AuthoritativeRulebook"))
        [NotMapped]
        public bool? IsDiscussionInAuthoritativeModel
        {
            get => F.AsBool(F.Memo(this, "IsDiscussionInAuthoritativeModel", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.AnnotationKind)), F.S("Comment"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.AnnotationKind)), F.S("Question"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.AnnotationKind)), F.S("Suggestion"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.AnnotationKind)), F.S("AlternativeInterpretation"))))), F.Bool3(F.Eq(F.Nullif(F.Of(this.StoredIn)), F.S("AuthoritativeRulebook")))))); set { }
        }

        // Formula FlagBypassedAnnotationInterface (rulebook: =AND({{AnnotationKind}} = "FlagOutdated", {{EnteredThrough}} <> "AnnotationPanel"))
        [NotMapped]
        public bool? FlagBypassedAnnotationInterface
        {
            get => F.AsBool(F.Memo(this, "FlagBypassedAnnotationInterface", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AnnotationKind)), F.S("FlagOutdated"))), F.Bool3(F.Ne(F.Nullif(F.Of(this.EnteredThrough)), F.S("AnnotationPanel")))))); set { }
        }

        // Formula IsLostNewKnowledge (rulebook: =AND({{AnnotationKind}} = "NewKnowledge", {{Status}} <> "Declined", {{PromotedToFragment}} = ""))
        [NotMapped]
        public bool? IsLostNewKnowledge
        {
            get => F.AsBool(F.Memo(this, "IsLostNewKnowledge", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AnnotationKind)), F.S("NewKnowledge"))), F.Bool3(F.Ne(F.Nullif(F.Of(this.Status)), F.S("Declined"))), F.Bool3(F.IsBlank(F.Of(this.PromotedToFragment)))))); set { }
        }

        // Formula IsFrictionWithoutGap (rulebook: =AND({{AnnotationKind}} = "MissingKnowledge", {{RaisedKnowledgeGap}} = ""))
        [NotMapped]
        public bool? IsFrictionWithoutGap
        {
            get => F.AsBool(F.Memo(this, "IsFrictionWithoutGap", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AnnotationKind)), F.S("MissingKnowledge"))), F.Bool3(F.IsBlank(F.Of(this.RaisedKnowledgeGap)))))); set { }
        }

        // Formula IsOpenOutdatedFlag (rulebook: =AND({{AnnotationKind}} = "FlagOutdated", {{Status}} = "Open"))
        [NotMapped]
        public bool? IsOpenOutdatedFlag
        {
            get => F.AsBool(F.Memo(this, "IsOpenOutdatedFlag", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AnnotationKind)), F.S("FlagOutdated"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Open")))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? AnnotatedByAgent { get; set; }
        public string? LifecycleStage { get; set; }
        public string? PromotedToFragment { get; set; }
        public string? RaisedKnowledgeGap { get; set; }

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

        private Agent _agent;

        [ForeignKey("AnnotatedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(AnnotatedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. AnnotatedByAgent: " + AnnotatedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(AnnotatedByAgent);
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
                        AnnotatedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private EncodingLifecycleStage _encodingLifecycleStage;

        [ForeignKey("LifecycleStage")]
        public virtual EncodingLifecycleStage EncodingLifecycleStage
        {
            get
            {
                if (_encodingLifecycleStage == null && !string.IsNullOrEmpty(LifecycleStage))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EncodingLifecycleStage - no database context is set. LifecycleStage: " + LifecycleStage + ".");
                        }
                        return null;
                    }
                    _encodingLifecycleStage = base.SoAContext.EncodingLifecycleStages.Find(LifecycleStage);
                    if (_encodingLifecycleStage != null)
                    {
                        base.SoAContext.Attach(_encodingLifecycleStage);
                    }
                }
                return _encodingLifecycleStage;
            }
            set
            {
                if (_encodingLifecycleStage != value)
                {
                    _encodingLifecycleStage = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_encodingLifecycleStage != null)
                    {
                        LifecycleStage = _encodingLifecycleStage.EncodingLifecycleStageId;
                    }
                }
            }
        }

        private KnowledgeFragment _knowledgeFragment;

        [ForeignKey("PromotedToFragment")]
        public virtual KnowledgeFragment KnowledgeFragment
        {
            get
            {
                if (_knowledgeFragment == null && !string.IsNullOrEmpty(PromotedToFragment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragment - no database context is set. PromotedToFragment: " + PromotedToFragment + ".");
                        }
                        return null;
                    }
                    _knowledgeFragment = base.SoAContext.KnowledgeFragments.Find(PromotedToFragment);
                    if (_knowledgeFragment != null)
                    {
                        base.SoAContext.Attach(_knowledgeFragment);
                    }
                }
                return _knowledgeFragment;
            }
            set
            {
                if (_knowledgeFragment != value)
                {
                    _knowledgeFragment = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeFragment != null)
                    {
                        PromotedToFragment = _knowledgeFragment.KnowledgeFragmentId;
                    }
                }
            }
        }

        private KnowledgeGap _knowledgeGap;

        [ForeignKey("RaisedKnowledgeGap")]
        public virtual KnowledgeGap KnowledgeGap
        {
            get
            {
                if (_knowledgeGap == null && !string.IsNullOrEmpty(RaisedKnowledgeGap))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeGap - no database context is set. RaisedKnowledgeGap: " + RaisedKnowledgeGap + ".");
                        }
                        return null;
                    }
                    _knowledgeGap = base.SoAContext.KnowledgeGaps.Find(RaisedKnowledgeGap);
                    if (_knowledgeGap != null)
                    {
                        base.SoAContext.Attach(_knowledgeGap);
                    }
                }
                return _knowledgeGap;
            }
            set
            {
                if (_knowledgeGap != value)
                {
                    _knowledgeGap = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeGap != null)
                    {
                        RaisedKnowledgeGap = _knowledgeGap.KnowledgeGapId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.StepRef;
            _ = this.Agent;
            _ = this.EncodingLifecycleStage;
            _ = this.KnowledgeFragment;
            _ = this.KnowledgeGap;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
