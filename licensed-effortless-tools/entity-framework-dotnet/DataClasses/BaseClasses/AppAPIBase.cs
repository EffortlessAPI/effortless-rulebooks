
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AppAPIs")]
    public class AppAPIBase : SoAEntityBase
    {
        [Key]
        public string ApiId { get; set; }

        public string Method { get; set; }
        public string Path { get; set; }
        public string Resource { get; set; }
        public string Action { get; set; }
        public bool WritesThrough { get; set; }
        public string Description { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.ApiId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
