
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
    [Table("HelloWhos")]
    public class HelloWhoBase : SoAEntityBase
    {
        [Key]
        public string HelloWhoId { get; set; }

        public string Name { get; set; }
        // Formula Introduction (rulebook: ="Hi, " & {{Name}} & ".")
        [NotMapped]
        public string? Introduction
        {
            get => F.AsString(F.Memo(this, "Introduction", () => F.Concat(F.S("Hi, "), F.TextOr(F.Of(this.Name)), F.S(".")))); set { }
        }

        // Formula IsBob (rulebook: =OR({{Name}}="Bob", {{Name}}="Bobby", {{Name}}="Robert"))
        [NotMapped]
        public bool? IsBob
        {
            get => F.AsBool(F.Memo(this, "IsBob", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Name)), F.S("Bob"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Name)), F.S("Bobby"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Name)), F.S("Robert")))))); set { }
        }




        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
