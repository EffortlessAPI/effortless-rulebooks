
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
    [Table("WorkflowViewDivergences")]
    public class WorkflowViewDivergenceBase : SoAEntityBase
    {
        [Key]
        public string WorkflowViewDivergenceId { get; set; }

        // Formula Name (rulebook: ={{Step}} & ": " & LEFT({{ViewA}}, 40) & " / " & LEFT({{ViewB}}, 40))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(": "), F.Text(F.Left(F.Of(this.ViewA), F.I(40))), F.S(" / "), F.Text(F.Left(F.Of(this.ViewB), F.I(40)))))); set { }
        }

        public string? ViewA { get; set; }
        public string? ViewB { get; set; }
        public string? ReconciledStatement { get; set; }
        public DateTimeOffset? ReconciledAt { get; set; }
        // Formula IsReconciled (rulebook: ={{ReconciledStatement}} <> "")
        [NotMapped]
        public bool? IsReconciled
        {
            get => F.AsBool(F.Memo(this, "IsReconciled", () => F.IsNotBlank(F.Of(this.ReconciledStatement)))); set { }
        }

        // Formula IsSurfacedButUnreconciled (rulebook: =AND({{ViewA}} <> "", {{ViewB}} <> "", {{ReconciledStatement}} = ""))
        [NotMapped]
        public bool? IsSurfacedButUnreconciled
        {
            get => F.AsBool(F.Memo(this, "IsSurfacedButUnreconciled", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ViewA))), F.Bool3(F.IsNotBlank(F.Of(this.ViewB))), F.Bool3(F.IsBlank(F.Of(this.ReconciledStatement)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ElicitationSession { get; set; }
        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? HolderA { get; set; }
        public string? HolderB { get; set; }
        public string? ReconciledIntoFragment { get; set; }

        private ElicitationSession _elicitationSessionRef;

        [ForeignKey("ElicitationSession")]
        public virtual ElicitationSession ElicitationSessionRef
        {
            get
            {
                if (_elicitationSessionRef == null && !string.IsNullOrEmpty(ElicitationSession))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ElicitationSessionRef - no database context is set. ElicitationSession: " + ElicitationSession + ".");
                        }
                        return null;
                    }
                    _elicitationSessionRef = base.SoAContext.ElicitationSessions.Find(ElicitationSession);
                    if (_elicitationSessionRef != null)
                    {
                        base.SoAContext.Attach(_elicitationSessionRef);
                    }
                }
                return _elicitationSessionRef;
            }
            set
            {
                if (_elicitationSessionRef != value)
                {
                    _elicitationSessionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_elicitationSessionRef != null)
                    {
                        ElicitationSession = _elicitationSessionRef.ElicitationSessionId;
                    }
                }
            }
        }

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

        [ForeignKey("HolderA")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(HolderA))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. HolderA: " + HolderA + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(HolderA);
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
                        HolderA = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("HolderB")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(HolderB))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. HolderB: " + HolderB + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(HolderB);
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
                        HolderB = _agentRef.AgentId;
                    }
                }
            }
        }

        private KnowledgeFragment _knowledgeFragment;

        [ForeignKey("ReconciledIntoFragment")]
        public virtual KnowledgeFragment KnowledgeFragment
        {
            get
            {
                if (_knowledgeFragment == null && !string.IsNullOrEmpty(ReconciledIntoFragment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragment - no database context is set. ReconciledIntoFragment: " + ReconciledIntoFragment + ".");
                        }
                        return null;
                    }
                    _knowledgeFragment = base.SoAContext.KnowledgeFragments.Find(ReconciledIntoFragment);
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
                        ReconciledIntoFragment = _knowledgeFragment.KnowledgeFragmentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ElicitationSessionRef;
            _ = this.ProcedureVersionRef;
            _ = this.StepRef;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.KnowledgeFragment;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
