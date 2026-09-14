
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
    [Table("JwtClaimMappings")]
    public class JwtClaimMappingBase : SoAEntityBase
    {
        [Key]
        public string JwtClaimMappingId { get; set; }

        // Formula Name (rulebook: ={{ClaimName}} & " -> " & {{SqlAccessor}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ClaimName)), F.S(" -> "), F.Text(F.Of(this.SqlAccessor))))); set { }
        }

        public string? ClaimName { get; set; }
        public string? SqlAccessor { get; set; }
        public bool? IsReservedClaim { get; set; }
        public bool? MapsToPrincipal { get; set; }
        public string? Description2 { get; set; }
        // Formula UsageCount (rulebook: =COUNTIFS(AccessPolicies!{{RowPredicate}}, {{SqlAccessor}}))
        [NotMapped]
        public decimal? UsageCount
        {
            get => F.AsDecimal(F.Memo(this, "UsageCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AccessPolicy>(base.SoAContext, "AccessPolicies", __c => __c.AccessPolicies), __r => F.CritField(F.Of(__r.RowPredicate), F.Of(this.SqlAccessor)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
