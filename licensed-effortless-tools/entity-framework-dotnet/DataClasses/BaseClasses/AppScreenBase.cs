
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AppScreens")]
    public class AppScreenBase : SoAEntityBase
    {
        [Key]
        public string ScreenId { get; set; }

        public string Path { get; set; }
        public string Title { get; set; }
        public string? ReadsEntities { get; set; }
        public string? WritesEntities { get; set; }
        // Formula MinRoleId
        public string MinRoleId
        {
            get => default /* TODO: translate formula:  */; set { }
        }

        public string? Layout { get; set; }
        public string? PrimaryAction { get; set; }
        public string? Story { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.ScreenId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
