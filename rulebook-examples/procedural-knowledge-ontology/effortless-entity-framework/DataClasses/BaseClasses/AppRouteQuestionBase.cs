
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AppRouteQuestions")]
    public class AppRouteQuestionBase : SoAEntityBase
    {
        [Key]
        public string AppRouteQuestionId { get; set; }

        // Formula Name (rulebook: ={{Route}} & " answers " & {{Question}})
        public string? Name
        {
            get => this.Route + " answers " + this.Question; set { }
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRoute - no database context is set. Route: " + Route + ".");
                        }
                        return null;
                    }
                    _appRoute = Context.AppRoutes.Find(Route);
                    if (_appRoute != null)
                    {
                        Context.Attach(_appRoute);
                    }
                }
                return _appRoute;
            }
            set
            {
                if (_appRoute != value)
                {
                    _appRoute = value;
                    Route = _appRoute == null ? default : _appRoute.AppRouteId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestion - no database context is set. Question: " + Question + ".");
                        }
                        return null;
                    }
                    _roleQuestion = Context.RoleQuestions.Find(Question);
                    if (_roleQuestion != null)
                    {
                        Context.Attach(_roleQuestion);
                    }
                }
                return _roleQuestion;
            }
            set
            {
                if (_roleQuestion != value)
                {
                    _roleQuestion = value;
                    Question = _roleQuestion == null ? default : _roleQuestion.RoleQuestionId;
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
