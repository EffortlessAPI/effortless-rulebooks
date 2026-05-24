
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("TestingFramework")]
    public class TestingFrameworkBase : SoAEntityBase
    {
        [Key]
        public string TestId { get; set; }

        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Purpose { get; set; }
        public string? Scope { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
