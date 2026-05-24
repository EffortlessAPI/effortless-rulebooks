
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AppNavigation")]
    public class AppNavigationBase : SoAEntityBase
    {
        [Key]
        public string NavId { get; set; }

        // Formula ParentNavId
        public string? ParentNavId
        {
            get => default /* TODO: translate formula:  */; set { }
        }

        public string Label { get; set; }
        public string? Icon { get; set; }
        // Formula ScreenId
        public string? ScreenId
        {
            get => default /* TODO: translate formula:  */; set { }
        }

        // Formula MinRoleId
        public string MinRoleId
        {
            get => default /* TODO: translate formula:  */; set { }
        }

        public decimal Order { get; set; }
        public string? StoryBeat { get; set; }



        protected override void LazyLoadProperties()
        {
        }

        public override string ToString()
        {
            return this.NavId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
