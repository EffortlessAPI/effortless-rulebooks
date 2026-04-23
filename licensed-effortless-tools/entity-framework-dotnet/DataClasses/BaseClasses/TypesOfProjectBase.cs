
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("TypesOfProject")]
    public class TypesOfProjectBase : SoAEntityBase
    {
        [Key]
        public string TypesOfProjectId { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? RequiresManagerApproval { get; set; }
        // Formula CountOfProjects (rulebook: =COUNTIFS(Projects!{{ProjectType}}, TypesOfProject!{{TypesOfProjectId}}))
        public int? CountOfProjects
        {
            get => this.Projects == null ? 0 : this.Projects.Count; set { }
        }


        public string? Projects { get; set; }

        private Project _project;

        [ForeignKey("Projects")]
        public virtual Project Project
        {
            get
            {
                if (_project == null && !string.IsNullOrEmpty(Projects))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Project - no database context is set. Projects: " + Projects + ".");
                        }
                        return null;
                    }
                    _project = Context.Projects.Find(Projects);
                    if (_project != null)
                    {
                        Context.Attach(_project);
                    }
                }
                return _project;
            }
            set
            {
                if (_project != value)
                {
                    _project = value;
                    Projects = _project == null ? default : _project.ProjectId;
                }
            }
        }

        private ObservableCollection<Project> _projects;

        [InverseProperty("TypesOfProject")]
        public virtual ObservableCollection<Project> Projects
        {
            get
            {
                if (_projects == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Projects - no database context is set. TypesOfProjectId: " + this.TypesOfProjectId + ".");
                        }
                        _projects = new ObservableCollection<Project>();
                    }
                    else
                    {
                        var items = Context.Projects.Where(x => x.ProjectType == this.TypesOfProjectId).ToList<Project>();
                        _projects = new ObservableCollection<Project>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _projects.CollectionChanged += Projects_CollectionChanged;
                }
                return _projects;
            }
            private set
            {
                if (_projects != null)
                {
                    _projects.CollectionChanged -= Projects_CollectionChanged;
                }
                _projects = value;
                if (_projects != null)
                {
                    _projects.CollectionChanged += Projects_CollectionChanged;
                }
            }
        }

        private void Projects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Project>())
                {
                    item.ProjectType = this.TypesOfProjectId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Project;
            _ = this.Projects;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
