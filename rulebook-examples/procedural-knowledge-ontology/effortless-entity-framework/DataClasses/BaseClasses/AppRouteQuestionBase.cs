
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
    [Table("AppRouteQuestions")]
    public class AppRouteQuestionBase : SoAEntityBase
    {
        [Key]
        public string AppRouteQuestionId { get; set; }

        // Formula Name (rulebook: ={{Route}} & " answers " & {{Question}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Route)), F.S(" answers "), F.Text(F.Of(this.Question))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Route { get; set; }
        public string? Question { get; set; }

        private AppRoute _appRoute;

        [ForeignKey("Route")]
        public virtual AppRoute AppRoute
        {
            get
            {
                if (_appRoute == null && !string.IsNullOrEmpty(Route))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRoute - no database context is set. Route: " + Route + ".");
                        }
                        return null;
                    }
                    _appRoute = base.SoAContext.AppRoutes.Find(Route);
                    if (_appRoute != null)
                    {
                        base.SoAContext.Attach(_appRoute);
                    }
                }
                return _appRoute;
            }
            set
            {
                if (_appRoute != value)
                {
                    _appRoute = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_appRoute != null)
                    {
                        Route = _appRoute.AppRouteId;
                    }
                }
            }
        }

        private RoleQuestion _roleQuestion;

        [ForeignKey("Question")]
        public virtual RoleQuestion RoleQuestion
        {
            get
            {
                if (_roleQuestion == null && !string.IsNullOrEmpty(Question))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestion - no database context is set. Question: " + Question + ".");
                        }
                        return null;
                    }
                    _roleQuestion = base.SoAContext.RoleQuestions.Find(Question);
                    if (_roleQuestion != null)
                    {
                        base.SoAContext.Attach(_roleQuestion);
                    }
                }
                return _roleQuestion;
            }
            set
            {
                if (_roleQuestion != value)
                {
                    _roleQuestion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleQuestion != null)
                    {
                        Question = _roleQuestion.RoleQuestionId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AppRoute;
            _ = this.RoleQuestion;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
