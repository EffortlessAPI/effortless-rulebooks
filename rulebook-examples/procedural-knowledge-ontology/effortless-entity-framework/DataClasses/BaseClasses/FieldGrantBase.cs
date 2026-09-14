
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
    [Table("FieldGrants")]
    public class FieldGrantBase : SoAEntityBase
    {
        [Key]
        public string FieldGrantId { get; set; }

        // Formula Name (rulebook: ={{Principal}} & " -> " & {{TargetField}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Principal)), F.S(" -> "), F.Text(F.Of(this.TargetField))))); set { }
        }

        public bool? CanRead { get; set; }
        public bool? CanWrite { get; set; }
        public string? MaskStrategy { get; set; }
        // Formula FieldTable (rulebook: =INDEX(RulebookFields!{{TargetTable}}, MATCH({{TargetField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public string? FieldTable
        {
            get => F.AsString(F.Memo(this, "FieldTable", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.TargetField), __r => F.Of(__r.TargetTable), () => F.Of(new RulebookField().TargetTable)))); set { }
        }

        // Formula FieldName (rulebook: =INDEX(RulebookFields!{{FieldName}}, MATCH({{TargetField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public string? FieldName
        {
            get => F.AsString(F.Memo(this, "FieldName", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.TargetField), __r => F.Of(__r.FieldName), () => F.Of(new RulebookField().FieldName)))); set { }
        }

        // Formula FieldIsDerived (rulebook: =INDEX(RulebookFields!{{IsDerived}}, MATCH({{TargetField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public bool? FieldIsDerived
        {
            get => F.AsBool(F.Memo(this, "FieldIsDerived", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.TargetField), __r => F.Of(__r.IsDerived), () => F.Of(new RulebookField().IsDerived)))); set { }
        }

        // Formula IsWritableDerivedField (rulebook: =AND({{CanWrite}}, {{FieldIsDerived}}))
        [NotMapped]
        public bool? IsWritableDerivedField
        {
            get => F.AsBool(F.Memo(this, "IsWritableDerivedField", () => F.And(F.IsTrueV(F.Of(this.CanWrite)), F.Bool3(F.Of(this.FieldIsDerived))))); set { }
        }

        // Formula IsMasked (rulebook: =AND({{MaskStrategy}} <> "plain", {{MaskStrategy}} <> ""))
        [NotMapped]
        public bool? IsMasked
        {
            get => F.AsBool(F.Memo(this, "IsMasked", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.MaskStrategy)), F.S("plain"))), F.Bool3(F.IsNotBlank(F.Of(this.MaskStrategy)))))); set { }
        }

        // Formula GrantKeyWhenReadable (rulebook: =IF({{CanRead}}, {{Principal}} & "|" & {{FieldTable}}, ""))
        [NotMapped]
        public string? GrantKeyWhenReadable
        {
            get => F.AsString(F.Memo(this, "GrantKeyWhenReadable", () => (F.Truthy(F.IsTrueV(F.Of(this.CanRead))) ? F.Concat(F.Text(F.Of(this.Principal)), F.S("|"), F.Text(F.Of(this.FieldTable))) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Principal { get; set; }
        public string? TargetField { get; set; }

        private AccessPrincipal _accessPrincipal;

        [ForeignKey("Principal")]
        public virtual AccessPrincipal AccessPrincipal
        {
            get
            {
                if (_accessPrincipal == null && !string.IsNullOrEmpty(Principal))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPrincipal - no database context is set. Principal: " + Principal + ".");
                        }
                        return null;
                    }
                    _accessPrincipal = base.SoAContext.AccessPrincipals.Find(Principal);
                    if (_accessPrincipal != null)
                    {
                        base.SoAContext.Attach(_accessPrincipal);
                    }
                }
                return _accessPrincipal;
            }
            set
            {
                if (_accessPrincipal != value)
                {
                    _accessPrincipal = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_accessPrincipal != null)
                    {
                        Principal = _accessPrincipal.AccessPrincipalId;
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
            _ = this.AccessPrincipal;
            _ = this.RulebookField;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
