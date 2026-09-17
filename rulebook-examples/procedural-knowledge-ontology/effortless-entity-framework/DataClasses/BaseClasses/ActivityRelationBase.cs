
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
    [Table("ActivityRelations")]
    public class ActivityRelationBase : SoAEntityBase
    {
        [Key]
        public string ActivityRelationId { get; set; }

        // Formula Name (rulebook: ={{FromStep}} & " " & {{RelationType}} & " " & {{ToStep}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.FromStep)), F.S(" "), F.Text(F.Of(this.RelationType)), F.S(" "), F.Text(F.Of(this.ToStep))))); set { }
        }

        public string? Rationale { get; set; }
        // Formula RelationTypeIsDefined (rulebook: =INDEX(RelationTypes!{{IsDefined}}, MATCH({{RelationType}}, RelationTypes!{{RelationTypeId}}, 0)))
        [NotMapped]
        public bool? RelationTypeIsDefined
        {
            get => F.AsBool(F.Memo(this, "RelationTypeIsDefined", () => F.Lookup<RelationType>(this, "RelationTypes", "RelationTypeId", __c => __c.RelationTypes, __r => F.Of(__r.RelationTypeId), F.Of(this.RelationType), __r => F.Of(__r.IsDefined), () => F.Of(new RelationType().IsDefined)))); set { }
        }

        // Formula UsesUndefinedRelationType (rulebook: ={{RelationTypeIsDefined}} = FALSE)
        [NotMapped]
        public bool? UsesUndefinedRelationType
        {
            get => F.AsBool(F.Memo(this, "UsesUndefinedRelationType", () => F.Eq(F.Of(this.RelationTypeIsDefined), F.B(false)))); set { }
        }

        // Formula FromStepVersion (rulebook: =INDEX(Steps!{{ProcedureVersion}}, MATCH({{FromStep}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? FromStepVersion
        {
            get => F.AsString(F.Memo(this, "FromStepVersion", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.FromStep), __r => F.Of(__r.ProcedureVersion), () => F.Of(new Step().ProcedureVersion)))); set { }
        }

        // Formula OverlapsVersionKey (rulebook: =IF({{RelationType}} = "Overlaps", {{FromStepVersion}}, ""))
        [NotMapped]
        public string? OverlapsVersionKey
        {
            get => F.AsString(F.Memo(this, "OverlapsVersionKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.RelationType)), F.S("Overlaps")))) ? F.Of(this.FromStepVersion) : F.S("")))); set { }
        }

        // Formula EnablesVersionKey (rulebook: =IF({{RelationType}} = "Enables", {{FromStepVersion}}, ""))
        [NotMapped]
        public string? EnablesVersionKey
        {
            get => F.AsString(F.Memo(this, "EnablesVersionKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.RelationType)), F.S("Enables")))) ? F.Of(this.FromStepVersion) : F.S("")))); set { }
        }

        // Formula PreventsVersionKey (rulebook: =IF({{RelationType}} = "Prevents", {{FromStepVersion}}, ""))
        [NotMapped]
        public string? PreventsVersionKey
        {
            get => F.AsString(F.Memo(this, "PreventsVersionKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.RelationType)), F.S("Prevents")))) ? F.Of(this.FromStepVersion) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? FromStep { get; set; }
        public string? RelationType { get; set; }
        public string? ToStep { get; set; }

        private Step _step;

        [ForeignKey("FromStep")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(FromStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. FromStep: " + FromStep + ".");
                        }
                        return null;
                    }
                    _step = base.SoAContext.Steps.Find(FromStep);
                    if (_step != null)
                    {
                        base.SoAContext.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_step != null)
                    {
                        FromStep = _step.StepId;
                    }
                }
            }
        }

        private RelationType _relationTypeRef;

        [ForeignKey("RelationType")]
        public virtual RelationType RelationTypeRef
        {
            get
            {
                if (_relationTypeRef == null && !string.IsNullOrEmpty(RelationType))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RelationTypeRef - no database context is set. RelationType: " + RelationType + ".");
                        }
                        return null;
                    }
                    _relationTypeRef = base.SoAContext.RelationTypes.Find(RelationType);
                    if (_relationTypeRef != null)
                    {
                        base.SoAContext.Attach(_relationTypeRef);
                    }
                }
                return _relationTypeRef;
            }
            set
            {
                if (_relationTypeRef != value)
                {
                    _relationTypeRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_relationTypeRef != null)
                    {
                        RelationType = _relationTypeRef.RelationTypeId;
                    }
                }
            }
        }

        private Step _stepRef;

        [ForeignKey("ToStep")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(ToStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. ToStep: " + ToStep + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(ToStep);
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
                        ToStep = _stepRef.StepId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Step;
            _ = this.RelationTypeRef;
            _ = this.StepRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
