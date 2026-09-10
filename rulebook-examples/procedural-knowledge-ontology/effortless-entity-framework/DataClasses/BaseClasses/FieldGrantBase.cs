
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("FieldGrants")]
    public class FieldGrantBase : SoAEntityBase
    {
        [Key]
        public string FieldGrantId { get; set; }

        // Formula Name (rulebook: ={{Principal}} & " -> " & {{TargetField}})
        public string? Name
        {
            get => this.Principal + " -> " + this.TargetField; set { }
        }

        public bool? CanRead { get; set; }
        public bool? CanWrite { get; set; }
        public string? MaskStrategy { get; set; }
        // Formula FieldTable (rulebook: =INDEX(RulebookFields!{{TargetTable}}, MATCH({{TargetField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        public string? FieldTable
        {
            get => INDEX(RulebookFields!this.TargetTable, MATCH(this.TargetField, RulebookFields!this.RulebookFieldId, 0)); set { }
        }

        // Formula FieldName (rulebook: =INDEX(RulebookFields!{{FieldName}}, MATCH({{TargetField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        public string? FieldName
        {
            get => INDEX(RulebookFields!this.FieldName, MATCH(this.TargetField, RulebookFields!this.RulebookFieldId, 0)); set { }
        }

        // Formula FieldIsDerived (rulebook: =INDEX(RulebookFields!{{IsDerived}}, MATCH({{TargetField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        public bool? FieldIsDerived
        {
            get => INDEX(RulebookFields!this.IsDerived, MATCH(this.TargetField, RulebookFields!this.RulebookFieldId, 0)); set { }
        }

        // Formula IsWritableDerivedField (rulebook: =AND({{CanWrite}}, {{FieldIsDerived}}))
        public bool? IsWritableDerivedField
        {
            get => AND(this.CanWrite, this.FieldIsDerived); set { }
        }

        // Formula IsMasked (rulebook: =AND({{MaskStrategy}} <> "plain", {{MaskStrategy}} <> ""))
        public bool? IsMasked
        {
            get => AND(this.MaskStrategy <> "plain", this.MaskStrategy <> ""); set { }
        }

        // Formula GrantKeyWhenReadable (rulebook: =IF({{CanRead}}, {{Principal}} & "|" & {{FieldTable}}, ""))
        public string? GrantKeyWhenReadable
        {
            get => IF(this.CanRead, this.Principal + "|" + this.FieldTable, ""); set { }
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPrincipal - no database context is set. Principal: " + Principal + ".");
                        }
                        return null;
                    }
                    _accessPrincipal = Context.AccessPrincipals.Find(Principal);
                    if (_accessPrincipal != null)
                    {
                        Context.Attach(_accessPrincipal);
                    }
                }
                return _accessPrincipal;
            }
            set
            {
                if (_accessPrincipal != value)
                {
                    _accessPrincipal = value;
                    Principal = _accessPrincipal == null ? default : _accessPrincipal.AccessPrincipalId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookField - no database context is set. TargetField: " + TargetField + ".");
                        }
                        return null;
                    }
                    _rulebookField = Context.RulebookFields.Find(TargetField);
                    if (_rulebookField != null)
                    {
                        Context.Attach(_rulebookField);
                    }
                }
                return _rulebookField;
            }
            set
            {
                if (_rulebookField != value)
                {
                    _rulebookField = value;
                    TargetField = _rulebookField == null ? default : _rulebookField.RulebookFieldId;
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
