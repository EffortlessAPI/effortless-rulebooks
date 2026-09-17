
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
    [Table("KnowledgeConversions")]
    public class KnowledgeConversionBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeConversionId { get; set; }

        // Formula Name (rulebook: ={{ConversionMode}} & ": " & LEFT({{Description}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ConversionMode)), F.S(": "), F.Text(F.Left(F.Of(this.Description), F.I(60)))))); set { }
        }

        public string? ConversionMode { get; set; }
        public string? FromForm { get; set; }
        public string? ToForm { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? OccurredAt { get; set; }
        // Formula IsModeInconsistentWithForms (rulebook: =NOT(OR(AND({{ConversionMode}} = "Socialization", {{FromForm}} = "Tacit", {{ToForm}} = "Tacit"), AND({{ConversionMode}} = "Externalization", {{FromForm}} = "Tacit", {{ToForm}} = "Explicit"), AND({{ConversionMode}} = "Combination", {{FromForm}} = "Explicit", {{ToForm}} = "Explicit"), AND({{ConversionMode}} = "Internalization", {{FromForm}} = "Explicit", {{ToForm}} = "Tacit"))))
        [NotMapped]
        public bool? IsModeInconsistentWithForms
        {
            get => F.AsBool(F.Memo(this, "IsModeInconsistentWithForms", () => F.Not(F.Bool3(F.Or(F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ConversionMode)), F.S("Socialization"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.FromForm)), F.S("Tacit"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ToForm)), F.S("Tacit"))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ConversionMode)), F.S("Externalization"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.FromForm)), F.S("Tacit"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ToForm)), F.S("Explicit"))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ConversionMode)), F.S("Combination"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.FromForm)), F.S("Explicit"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ToForm)), F.S("Explicit"))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ConversionMode)), F.S("Internalization"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.FromForm)), F.S("Explicit"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ToForm)), F.S("Tacit")))))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ElicitationSession { get; set; }
        public string? ResultFragment { get; set; }

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

        private KnowledgeFragment _knowledgeFragment;

        [ForeignKey("ResultFragment")]
        public virtual KnowledgeFragment KnowledgeFragment
        {
            get
            {
                if (_knowledgeFragment == null && !string.IsNullOrEmpty(ResultFragment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragment - no database context is set. ResultFragment: " + ResultFragment + ".");
                        }
                        return null;
                    }
                    _knowledgeFragment = base.SoAContext.KnowledgeFragments.Find(ResultFragment);
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
                        ResultFragment = _knowledgeFragment.KnowledgeFragmentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ElicitationSessionRef;
            _ = this.KnowledgeFragment;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
