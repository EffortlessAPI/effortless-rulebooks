
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
    [Table("RoleSchemaViews")]
    public class RoleSchemaViewBase : SoAEntityBase
    {
        [Key]
        public string RoleSchemaViewId { get; set; }

        // Formula Name (rulebook: ={{SchemaName}} & "." & {{ViewName}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.SchemaName)), F.S("."), F.Text(F.Of(this.ViewName))))); set { }
        }

        public string? ViewName { get; set; }
        // Formula SchemaName (rulebook: =INDEX(RoleSchemas!{{SchemaName}}, MATCH({{RoleSchema}}, RoleSchemas!{{RoleSchemaId}}, 0)))
        [NotMapped]
        public string? SchemaName
        {
            get => F.AsString(F.Memo(this, "SchemaName", () => F.Lookup<RoleSchema>(this, "RoleSchemas", "RoleSchemaId", __c => __c.RoleSchemas, __r => F.Of(__r.RoleSchemaId), F.Of(this.RoleSchema), __r => F.Of(__r.SchemaName), () => F.Of(new RoleSchema().SchemaName)))); set { }
        }

        // Formula SourceView (rulebook: =INDEX(RulebookTables!{{PhysicalView}}, MATCH({{TargetTable}}, RulebookTables!{{RulebookTableId}}, 0)))
        [NotMapped]
        public string? SourceView
        {
            get => F.AsString(F.Memo(this, "SourceView", () => F.Lookup<RulebookTable>(this, "RulebookTables", "RulebookTableId", __c => __c.RulebookTables, __r => F.Of(__r.RulebookTableId), F.Of(this.TargetTable), __r => F.Of(__r.PhysicalView), () => F.Of(new RulebookTable().PhysicalView)))); set { }
        }

        // Formula GrantKey (rulebook: ={{Principal}} & "|" & {{TargetTable}})
        [NotMapped]
        public string? GrantKey
        {
            get => F.AsString(F.Memo(this, "GrantKey", () => F.Concat(F.Text(F.Of(this.Principal)), F.S("|"), F.Text(F.Of(this.TargetTable))))); set { }
        }

        // Formula ColumnCount (rulebook: =COUNTIFS(FieldGrants!{{GrantKeyWhenReadable}}, {{GrantKey}}))
        [NotMapped]
        public decimal? ColumnCount
        {
            get => F.AsDecimal(F.Memo(this, "ColumnCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<FieldGrant>(base.SoAContext, "FieldGrants", __c => __c.FieldGrants), __r => F.CritField(F.Of(__r.GrantKeyWhenReadable), F.Of(this.GrantKey)))))); set { }
        }

        // Formula TableFieldCount (rulebook: =INDEX(RulebookTables!{{FieldCount}}, MATCH({{TargetTable}}, RulebookTables!{{RulebookTableId}}, 0)))
        [NotMapped]
        public decimal? TableFieldCount
        {
            get => F.AsDecimal(F.Memo(this, "TableFieldCount", () => F.Lookup<RulebookTable>(this, "RulebookTables", "RulebookTableId", __c => __c.RulebookTables, __r => F.Of(__r.RulebookTableId), F.Of(this.TargetTable), __r => F.Of(__r.FieldCount), () => F.Of(new RulebookTable().FieldCount)))); set { }
        }

        // Formula IsFullWidth (rulebook: =AND({{ColumnCount}} > 0, {{ColumnCount}} >= {{TableFieldCount}}))
        [NotMapped]
        public bool? IsFullWidth
        {
            get => F.AsBool(F.Memo(this, "IsFullWidth", () => F.And(F.Bool3(F.Cmp(F.Of(this.ColumnCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.ColumnCount), ">=", F.Of(this.TableFieldCount)))))); set { }
        }

        // Formula IsDegenerateView (rulebook: ={{ColumnCount}} = 0)
        [NotMapped]
        public bool? IsDegenerateView
        {
            get => F.AsBool(F.Memo(this, "IsDegenerateView", () => F.Eq(F.Of(this.ColumnCount), F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? RoleSchema { get; set; }
        public string? Principal { get; set; }
        public string? TargetTable { get; set; }

        private RoleSchema _roleSchemaRef;

        [ForeignKey("RoleSchema")]
        public virtual RoleSchema RoleSchemaRef
        {
            get
            {
                if (_roleSchemaRef == null && !string.IsNullOrEmpty(RoleSchema))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleSchemaRef - no database context is set. RoleSchema: " + RoleSchema + ".");
                        }
                        return null;
                    }
                    _roleSchemaRef = base.SoAContext.RoleSchemas.Find(RoleSchema);
                    if (_roleSchemaRef != null)
                    {
                        base.SoAContext.Attach(_roleSchemaRef);
                    }
                }
                return _roleSchemaRef;
            }
            set
            {
                if (_roleSchemaRef != value)
                {
                    _roleSchemaRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleSchemaRef != null)
                    {
                        RoleSchema = _roleSchemaRef.RoleSchemaId;
                    }
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

        private RulebookTable _rulebookTable;

        [ForeignKey("TargetTable")]
        public virtual RulebookTable RulebookTable
        {
            get
            {
                if (_rulebookTable == null && !string.IsNullOrEmpty(TargetTable))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTable - no database context is set. TargetTable: " + TargetTable + ".");
                        }
                        return null;
                    }
                    _rulebookTable = base.SoAContext.RulebookTables.Find(TargetTable);
                    if (_rulebookTable != null)
                    {
                        base.SoAContext.Attach(_rulebookTable);
                    }
                }
                return _rulebookTable;
            }
            set
            {
                if (_rulebookTable != value)
                {
                    _rulebookTable = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookTable != null)
                    {
                        TargetTable = _rulebookTable.RulebookTableId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RoleSchemaRef;
            _ = this.AccessPrincipal;
            _ = this.RulebookTable;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
