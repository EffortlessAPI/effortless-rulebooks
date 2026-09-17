
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
    [Table("InterviewProbes")]
    public class InterviewProbeBase : SoAEntityBase
    {
        [Key]
        public string InterviewProbeId { get; set; }

        // Formula Name (rulebook: ={{ProbeKind}} & ": " & LEFT({{Prompt}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProbeKind)), F.S(": "), F.Text(F.Left(F.Of(this.Prompt), F.I(60)))))); set { }
        }

        public string? ProbeKind { get; set; }
        public string? Prompt { get; set; }
        public string? Answer { get; set; }
        // Formula WhyAnswer (rulebook: =IF({{ProbeKind}} = "Why", {{Answer}}, ""))
        [NotMapped]
        public string? WhyAnswer
        {
            get => F.AsString(F.Memo(this, "WhyAnswer", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.ProbeKind)), F.S("Why")))) ? F.Of(this.Answer) : F.S("")))); set { }
        }

        // Formula ShortfallAnswer (rulebook: =IF({{ProbeKind}} = "ProcedureFallsShort", {{Answer}}, ""))
        [NotMapped]
        public string? ShortfallAnswer
        {
            get => F.AsString(F.Memo(this, "ShortfallAnswer", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.ProbeKind)), F.S("ProcedureFallsShort")))) ? F.Of(this.Answer) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ElicitationSession { get; set; }
        public string? Step { get; set; }

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


        protected override void LazyLoadProperties()
        {
            _ = this.ElicitationSessionRef;
            _ = this.StepRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
