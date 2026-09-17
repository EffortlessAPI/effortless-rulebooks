
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
    [Table("ExpectedInferenceChecks")]
    public class ExpectedInferenceCheckBase : SoAEntityBase
    {
        [Key]
        public string ExpectedInferenceCheckId { get; set; }

        // Formula Name (rulebook: ={{ChangeValidationRun}} & ": " & LEFT({{ChainDescription}}, 50))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ChangeValidationRun)), F.S(": "), F.Text(F.Left(F.Of(this.ChainDescription), F.I(50)))))); set { }
        }

        public string? ChainDescription { get; set; }
        public string? ExpectedValue { get; set; }
        public string? ProducedValue { get; set; }
        // Formula IsUnproduced (rulebook: ={{ProducedValue}} <> {{ExpectedValue}})
        [NotMapped]
        public bool? IsUnproduced
        {
            get => F.AsBool(F.Memo(this, "IsUnproduced", () => F.Ne(F.Nullif(F.Of(this.ProducedValue)), F.Nullif(F.Of(this.ExpectedValue))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ChangeValidationRun { get; set; }
        public string? ExpectedField { get; set; }

        private ChangeValidationRun _changeValidationRunRef;

        [ForeignKey("ChangeValidationRun")]
        public virtual ChangeValidationRun ChangeValidationRunRef
        {
            get
            {
                if (_changeValidationRunRef == null && !string.IsNullOrEmpty(ChangeValidationRun))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeValidationRunRef - no database context is set. ChangeValidationRun: " + ChangeValidationRun + ".");
                        }
                        return null;
                    }
                    _changeValidationRunRef = base.SoAContext.ChangeValidationRuns.Find(ChangeValidationRun);
                    if (_changeValidationRunRef != null)
                    {
                        base.SoAContext.Attach(_changeValidationRunRef);
                    }
                }
                return _changeValidationRunRef;
            }
            set
            {
                if (_changeValidationRunRef != value)
                {
                    _changeValidationRunRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_changeValidationRunRef != null)
                    {
                        ChangeValidationRun = _changeValidationRunRef.ChangeValidationRunId;
                    }
                }
            }
        }

        private RulebookField _rulebookField;

        [ForeignKey("ExpectedField")]
        public virtual RulebookField RulebookField
        {
            get
            {
                if (_rulebookField == null && !string.IsNullOrEmpty(ExpectedField))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookField - no database context is set. ExpectedField: " + ExpectedField + ".");
                        }
                        return null;
                    }
                    _rulebookField = base.SoAContext.RulebookFields.Find(ExpectedField);
                    if (_rulebookField != null)
                    {
                        base.SoAContext.Attach(_rulebookField);
                    }
                }
                return _rulebookField;
            }
            set
            {
                if (_rulebookField != value)
                {
                    _rulebookField = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookField != null)
                    {
                        ExpectedField = _rulebookField.RulebookFieldId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ChangeValidationRunRef;
            _ = this.RulebookField;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
