
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("JwtClaimMappings")]
    public class JwtClaimMappingBase : SoAEntityBase
    {
        [Key]
        public string JwtClaimMappingId { get; set; }

        // Formula Name (rulebook: ={{ClaimName}} & " -> " & {{SqlAccessor}})
        public string? Name
        {
            get => this.ClaimName + " -> " + this.SqlAccessor; set { }
        }

        public string? ClaimName { get; set; }
        public string? SqlAccessor { get; set; }
        public bool? IsReservedClaim { get; set; }
        public bool? MapsToPrincipal { get; set; }
        public string? Description2 { get; set; }
        // Formula UsageCount (rulebook: =COUNTIFS(AccessPolicies!{{RowPredicate}}, {{SqlAccessor}}))
        public decimal? UsageCount
        {
            get => COUNTIFS(AccessPolicies!this.RowPredicate, this.SqlAccessor); set { }
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
