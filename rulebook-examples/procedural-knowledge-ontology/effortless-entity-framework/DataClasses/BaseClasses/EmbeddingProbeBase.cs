
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
    [Table("EmbeddingProbes")]
    public class EmbeddingProbeBase : SoAEntityBase
    {
        [Key]
        public string EmbeddingProbeId { get; set; }

        // Formula Name (rulebook: ={{TermA}} & " / " & {{TermB}} & " @ " & {{EmbeddingModel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.TermA)), F.S(" / "), F.Text(F.Of(this.TermB)), F.S(" @ "), F.Text(F.Of(this.EmbeddingModel))))); set { }
        }

        public string? TermA { get; set; }
        public string? TermB { get; set; }
        public string? ExpectedRelation { get; set; }
        public string? EmbeddingModel { get; set; }
        public decimal? CosineSimilarity { get; set; }
        public DateTimeOffset? ProbedAt { get; set; }
        // Formula SynonymsNotSimilar (rulebook: =AND({{ExpectedRelation}} = "NearSynonym", {{CosineSimilarity}} < 0.7))
        [NotMapped]
        public bool? SynonymsNotSimilar
        {
            get => F.AsBool(F.Memo(this, "SynonymsNotSimilar", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ExpectedRelation)), F.S("NearSynonym"))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.CosineSimilarity)), "<", F.D(0.7)))))); set { }
        }

        // Formula OppositesNotOpposed (rulebook: =AND({{ExpectedRelation}} = "Opposite", {{CosineSimilarity}} > 0.3))
        [NotMapped]
        public bool? OppositesNotOpposed
        {
            get => F.AsBool(F.Memo(this, "OppositesNotOpposed", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ExpectedRelation)), F.S("Opposite"))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.CosineSimilarity)), ">", F.D(0.3)))))); set { }
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
