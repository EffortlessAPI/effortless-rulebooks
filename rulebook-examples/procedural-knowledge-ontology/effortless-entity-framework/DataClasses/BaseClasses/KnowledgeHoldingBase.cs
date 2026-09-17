
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
    [Table("KnowledgeHoldings")]
    public class KnowledgeHoldingBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeHoldingId { get; set; }

        // Formula Name (rulebook: ={{Snapshot}} & " " & {{Carrier}} & ": " & LEFT({{Statement}}, 50))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Snapshot)), F.S(" "), F.Text(F.Of(this.Carrier)), F.S(": "), F.Text(F.Left(F.Of(this.Statement), F.I(50)))))); set { }
        }

        public string? Snapshot { get; set; }
        public string? KnowledgeForm { get; set; }
        public string? Carrier { get; set; }
        public string? Statement { get; set; }
        public bool? IsUnsaidInSop { get; set; }
        public bool? HolderIsVeteran { get; set; }
        public bool? HandlesExceptionOrDiscretion { get; set; }
        // Formula FormalizedFragmentSession (rulebook: =INDEX(KnowledgeFragments!{{ElicitationSession}}, MATCH({{FormalizedAs}}, KnowledgeFragments!{{KnowledgeFragmentId}}, 0)))
        [NotMapped]
        public string? FormalizedFragmentSession
        {
            get => F.AsString(F.Memo(this, "FormalizedFragmentSession", () => F.Lookup<KnowledgeFragment>(this, "KnowledgeFragments", "KnowledgeFragmentId", __c => __c.KnowledgeFragments, __r => F.Of(__r.KnowledgeFragmentId), F.Of(this.FormalizedAs), __r => F.Of(__r.ElicitationSession), () => F.Of(new KnowledgeFragment().ElicitationSession)))); set { }
        }

        // Formula IsUnformalizedProcessKnowledge (rulebook: =AND({{FormalizedAs}} = "", {{Carrier}} <> "ProcedureRulebook"))
        [NotMapped]
        public bool? IsUnformalizedProcessKnowledge
        {
            get => F.AsBool(F.Memo(this, "IsUnformalizedProcessKnowledge", () => F.And(F.Bool3(F.IsBlank(F.Of(this.FormalizedAs))), F.Bool3(F.Ne(F.Nullif(F.Of(this.Carrier)), F.S("ProcedureRulebook")))))); set { }
        }

        // Formula IsProceduralKnowledge (rulebook: ={{FormalizedAs}} <> "")
        [NotMapped]
        public bool? IsProceduralKnowledge
        {
            get => F.AsBool(F.Memo(this, "IsProceduralKnowledge", () => F.IsNotBlank(F.Of(this.FormalizedAs)))); set { }
        }

        // Formula IsJudgmentOutsideDocument (rulebook: =AND({{IsUnsaidInSop}}, {{IsUnformalizedProcessKnowledge}}))
        [NotMapped]
        public bool? IsJudgmentOutsideDocument
        {
            get => F.AsBool(F.Memo(this, "IsJudgmentOutsideDocument", () => F.And(F.IsTrueV(F.Of(this.IsUnsaidInSop)), F.Bool3(F.Of(this.IsUnformalizedProcessKnowledge))))); set { }
        }

        // Formula IsVeteranDiscretion (rulebook: =AND({{HolderIsVeteran}}, {{HandlesExceptionOrDiscretion}}, {{IsUnformalizedProcessKnowledge}}))
        [NotMapped]
        public bool? IsVeteranDiscretion
        {
            get => F.AsBool(F.Memo(this, "IsVeteranDiscretion", () => F.And(F.IsTrueV(F.Of(this.HolderIsVeteran)), F.IsTrueV(F.Of(this.HandlesExceptionOrDiscretion)), F.Bool3(F.Of(this.IsUnformalizedProcessKnowledge))))); set { }
        }

        // Formula IsFormalizedWithoutElicitationWork (rulebook: =AND({{FormalizedAs}} <> "", {{FormalizedFragmentSession}} = ""))
        [NotMapped]
        public bool? IsFormalizedWithoutElicitationWork
        {
            get => F.AsBool(F.Memo(this, "IsFormalizedWithoutElicitationWork", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.FormalizedAs))), F.Bool3(F.IsBlank(F.Of(this.FormalizedFragmentSession)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? HolderAgent { get; set; }
        public string? FormalizedAs { get; set; }

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

        [ForeignKey("HolderAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(HolderAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. HolderAgent: " + HolderAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(HolderAgent);
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
                        HolderAgent = _agent.AgentId;
                    }
                }
            }
        }

        private KnowledgeFragment _knowledgeFragment;

        [ForeignKey("FormalizedAs")]
        public virtual KnowledgeFragment KnowledgeFragment
        {
            get
            {
                if (_knowledgeFragment == null && !string.IsNullOrEmpty(FormalizedAs))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragment - no database context is set. FormalizedAs: " + FormalizedAs + ".");
                        }
                        return null;
                    }
                    _knowledgeFragment = base.SoAContext.KnowledgeFragments.Find(FormalizedAs);
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
                        FormalizedAs = _knowledgeFragment.KnowledgeFragmentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.StepRef;
            _ = this.Agent;
            _ = this.KnowledgeFragment;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
