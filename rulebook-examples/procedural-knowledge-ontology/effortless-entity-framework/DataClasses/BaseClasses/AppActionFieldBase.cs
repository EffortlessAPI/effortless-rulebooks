
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
    [Table("AppActionFields")]
    public class AppActionFieldBase : SoAEntityBase
    {
        [Key]
        public string AppActionFieldId { get; set; }

        // Formula Name (rulebook: ={{AppAction}} & " / " & {{FieldLabel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.AppAction)), F.S(" / "), F.Text(F.Of(this.FieldLabel))))); set { }
        }

        public string? FieldLabel { get; set; }
        public string? InputKind { get; set; }
        public string? FixedValue { get; set; }
        public string? ChoicesFrom { get; set; }
        public int? SortOrder { get; set; }
        // Formula TargetFieldType (rulebook: =INDEX(RulebookFields!{{FieldType}}, MATCH({{TargetField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public string? TargetFieldType
        {
            get => F.AsString(F.Memo(this, "TargetFieldType", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.TargetField), __r => F.Of(__r.FieldType), () => F.Of(new RulebookField().FieldType)))); set { }
        }

        // Formula WritesDerivedField (rulebook: =AND({{TargetField}} <> "", {{TargetFieldType}} <> "raw", {{TargetFieldType}} <> "relationship"))
        [NotMapped]
        public bool? WritesDerivedField
        {
            get => F.AsBool(F.Memo(this, "WritesDerivedField", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.TargetField))), F.Bool3(F.Ne(F.Of(this.TargetFieldType), F.S("raw"))), F.Bool3(F.Ne(F.Of(this.TargetFieldType), F.S("relationship")))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? AppAction { get; set; }
        public string? TargetField { get; set; }

        private AppAction _appActionRef;

        [ForeignKey("AppAction")]
        public virtual AppAction AppActionRef
        {
            get
            {
                if (_appActionRef == null && !string.IsNullOrEmpty(AppAction))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppActionRef - no database context is set. AppAction: " + AppAction + ".");
                        }
                        return null;
                    }
                    _appActionRef = base.SoAContext.AppActions.Find(AppAction);
                    if (_appActionRef != null)
                    {
                        base.SoAContext.Attach(_appActionRef);
                    }
                }
                return _appActionRef;
            }
            set
            {
                if (_appActionRef != value)
                {
                    _appActionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_appActionRef != null)
                    {
                        AppAction = _appActionRef.AppActionId;
                    }
                }
            }
        }

        private RulebookField _rulebookField;

        [ForeignKey("TargetField")]
        public virtual RulebookField RulebookField
        {
            get
            {
                if (_rulebookField == null && !string.IsNullOrEmpty(TargetField))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookField - no database context is set. TargetField: " + TargetField + ".");
                        }
                        return null;
                    }
                    _rulebookField = base.SoAContext.RulebookFields.Find(TargetField);
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
                        TargetField = _rulebookField.RulebookFieldId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AppActionRef;
            _ = this.RulebookField;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
