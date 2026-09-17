
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
    [Table("ObservedActions")]
    public class ObservedActionBase : SoAEntityBase
    {
        [Key]
        public string ObservedActionId { get; set; }

        // Formula Name (rulebook: ={{ActionKind}} & ": " & LEFT({{ActionDescription}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ActionKind)), F.S(": "), F.Text(F.Left(F.Of(this.ActionDescription), F.I(60)))))); set { }
        }

        public string? ActionDescription { get; set; }
        public string? ActionKind { get; set; }
        public bool? MentionedInOwnAccount { get; set; }
        public string? StatedReason { get; set; }
        public string? CounterfactualCondition { get; set; }
        public string? CounterfactualAnswer { get; set; }
        // Formula IsSmallChoice (rulebook: ={{ActionKind}} = "SmallChoice")
        [NotMapped]
        public bool? IsSmallChoice
        {
            get => F.AsBool(F.Memo(this, "IsSmallChoice", () => F.Eq(F.Nullif(F.Of(this.ActionKind)), F.S("SmallChoice")))); set { }
        }

        // Formula IsUnofficialWorkaround (rulebook: ={{ActionKind}} = "Workaround")
        [NotMapped]
        public bool? IsUnofficialWorkaround
        {
            get => F.AsBool(F.Memo(this, "IsUnofficialWorkaround", () => F.Eq(F.Nullif(F.Of(this.ActionKind)), F.S("Workaround")))); set { }
        }

        // Formula IsOmittedFromOwnAccount (rulebook: =AND({{ElicitationSession}} <> "", NOT({{MentionedInOwnAccount}})))
        [NotMapped]
        public bool? IsOmittedFromOwnAccount
        {
            get => F.AsBool(F.Memo(this, "IsOmittedFromOwnAccount", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ElicitationSession))), F.Bool3(F.Not(F.IsTrueV(F.Of(this.MentionedInOwnAccount))))))); set { }
        }

        // Formula IsMissedStepLeftUncaptured (rulebook: =AND({{IsOmittedFromOwnAccount}}, {{CapturedAsFragment}} = ""))
        [NotMapped]
        public bool? IsMissedStepLeftUncaptured
        {
            get => F.AsBool(F.Memo(this, "IsMissedStepLeftUncaptured", () => F.And(F.Bool3(F.Of(this.IsOmittedFromOwnAccount)), F.Bool3(F.IsBlank(F.Of(this.CapturedAsFragment)))))); set { }
        }

        // Formula IsWatchedNotQuestioned (rulebook: =AND(OR({{ActionKind}} = "SmallChoice", {{ActionKind}} = "Workaround"), {{StatedReason}} = ""))
        [NotMapped]
        public bool? IsWatchedNotQuestioned
        {
            get => F.AsBool(F.Memo(this, "IsWatchedNotQuestioned", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.ActionKind)), F.S("SmallChoice"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ActionKind)), F.S("Workaround"))))), F.Bool3(F.IsBlank(F.Of(this.StatedReason)))))); set { }
        }

        // Formula HasRecordedReason (rulebook: ={{StatedReason}} <> "")
        [NotMapped]
        public bool? HasRecordedReason
        {
            get => F.AsBool(F.Memo(this, "HasRecordedReason", () => F.IsNotBlank(F.Of(this.StatedReason)))); set { }
        }

        // Formula HasCounterfactualAnswer (rulebook: =AND({{CounterfactualCondition}} <> "", {{CounterfactualAnswer}} <> ""))
        [NotMapped]
        public bool? HasCounterfactualAnswer
        {
            get => F.AsBool(F.Memo(this, "HasCounterfactualAnswer", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.CounterfactualCondition))), F.Bool3(F.IsNotBlank(F.Of(this.CounterfactualAnswer)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ElicitationSession { get; set; }
        public string? Step { get; set; }
        public string? Practitioner { get; set; }
        public string? CapturedAsFragment { get; set; }

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

        [ForeignKey("Practitioner")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(Practitioner))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. Practitioner: " + Practitioner + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(Practitioner);
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
                        Practitioner = _agent.AgentId;
                    }
                }
            }
        }

        private KnowledgeFragment _knowledgeFragment;

        [ForeignKey("CapturedAsFragment")]
        public virtual KnowledgeFragment KnowledgeFragment
        {
            get
            {
                if (_knowledgeFragment == null && !string.IsNullOrEmpty(CapturedAsFragment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragment - no database context is set. CapturedAsFragment: " + CapturedAsFragment + ".");
                        }
                        return null;
                    }
                    _knowledgeFragment = base.SoAContext.KnowledgeFragments.Find(CapturedAsFragment);
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
                        CapturedAsFragment = _knowledgeFragment.KnowledgeFragmentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ElicitationSessionRef;
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
