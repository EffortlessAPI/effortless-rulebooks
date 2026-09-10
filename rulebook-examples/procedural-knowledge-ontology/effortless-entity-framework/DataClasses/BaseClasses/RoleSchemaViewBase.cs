
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("RoleSchemaViews")]
    public class RoleSchemaViewBase : SoAEntityBase
    {
        [Key]
        public string RoleSchemaViewId { get; set; }

        // Formula Name (rulebook: ={{SchemaName}} & "." & {{ViewName}})
        public string? Name
        {
            get => this.SchemaName + "." + this.ViewName; set { }
        }

        public string? ViewName { get; set; }
        // Formula SchemaName (rulebook: =INDEX(RoleSchemas!{{SchemaName}}, MATCH({{RoleSchema}}, RoleSchemas!{{RoleSchemaId}}, 0)))
        public string? SchemaName
        {
            get => INDEX(RoleSchemas!this.SchemaName, MATCH(this.RoleSchema, RoleSchemas!this.RoleSchemaId, 0)); set { }
        }

        // Formula SourceView (rulebook: =INDEX(RulebookTables!{{PhysicalView}}, MATCH({{TargetTable}}, RulebookTables!{{RulebookTableId}}, 0)))
        public string? SourceView
        {
            get => INDEX(RulebookTables!this.PhysicalView, MATCH(this.TargetTable, RulebookTables!this.RulebookTableId, 0)); set { }
        }

        // Formula GrantKey (rulebook: ={{Principal}} & "|" & {{TargetTable}})
        public string? GrantKey
        {
            get => this.Principal + "|" + this.TargetTable; set { }
        }

        // Formula ColumnCount (rulebook: =COUNTIFS(FieldGrants!{{GrantKeyWhenReadable}}, {{GrantKey}}))
        public decimal? ColumnCount
        {
            get => COUNTIFS(FieldGrants!this.GrantKeyWhenReadable, this.GrantKey); set { }
        }

        // Formula TableFieldCount (rulebook: =INDEX(RulebookTables!{{FieldCount}}, MATCH({{TargetTable}}, RulebookTables!{{RulebookTableId}}, 0)))
        public decimal? TableFieldCount
        {
            get => INDEX(RulebookTables!this.FieldCount, MATCH(this.TargetTable, RulebookTables!this.RulebookTableId, 0)); set { }
        }

        // Formula IsFullWidth (rulebook: =AND({{ColumnCount}} > 0, {{ColumnCount}} >= {{TableFieldCount}}))
        public bool? IsFullWidth
        {
            get => AND(this.ColumnCount > 0, this.ColumnCount >= this.TableFieldCount); set { }
        }

        // Formula IsDegenerateView (rulebook: ={{ColumnCount}} = 0)
        public bool? IsDegenerateView
        {
            get => this.ColumnCount = 0; set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? RoleSchema { get; set; }
        public string? Principal { get; set; }
        public string? TargetTable { get; set; }

        private RoleSchema _roleSchema;

        [ForeignKey("RoleSchema")]
        public virtual RoleSchema RoleSchema
        {
            get
            {
                if (_roleSchema == null && !string.IsNullOrEmpty(RoleSchema))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleSchema - no database context is set. RoleSchema: " + RoleSchema + ".");
                        }
                        return null;
                    }
                    _roleSchema = Context.RoleSchemas.Find(RoleSchema);
                    if (_roleSchema != null)
                    {
                        Context.Attach(_roleSchema);
                    }
                }
                return _roleSchema;
            }
            set
            {
                if (_roleSchema != value)
                {
                    _roleSchema = value;
                    RoleSchema = _roleSchema == null ? default : _roleSchema.RoleSchemaId;
                }
            }
        }

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

        private RulebookTable _rulebookTable;

        [ForeignKey("TargetTable")]
        public virtual RulebookTable RulebookTable
        {
            get
            {
                if (_rulebookTable == null && !string.IsNullOrEmpty(TargetTable))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTable - no database context is set. TargetTable: " + TargetTable + ".");
                        }
                        return null;
                    }
                    _rulebookTable = Context.RulebookTables.Find(TargetTable);
                    if (_rulebookTable != null)
                    {
                        Context.Attach(_rulebookTable);
                    }
                }
                return _rulebookTable;
            }
            set
            {
                if (_rulebookTable != value)
                {
                    _rulebookTable = value;
                    TargetTable = _rulebookTable == null ? default : _rulebookTable.RulebookTableId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RoleSchema;
            _ = this.AccessPrincipal;
            _ = this.RulebookTable;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
