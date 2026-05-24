
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Employees")]
    public class EmployeeBase : SoAEntityBase
    {
        [Key]
        public string EmployeeId { get; set; }

        public string? Name { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        // Formula RoleName (rulebook: =INDEX(Roles!{{Name}}, MATCH(Employees!{{Role}}, Roles!{{RoleId}}, 0)))
        public string? RoleName
        {
            get => this.Role == null ? default : this.Role.Name; set { }
        }

        // Formula RoleDescription (rulebook: =INDEX(Roles!{{Description}}, MATCH(Employees!{{Role}}, Roles!{{RoleId}}, 0)))
        public string? RoleDescription
        {
            get => this.Role == null ? default : this.Role.Description; set { }
        }

        // Formula RoleIsManager (rulebook: =INDEX(Roles!{{IsManager}}, MATCH(Employees!{{Role}}, Roles!{{RoleId}}, 0)))
        public bool? RoleIsManager
        {
            get => this.Role == null ? default : this.Role.IsManager; set { }
        }


        public string? Role { get; set; }
        public string? Projects { get; set; }

        private Role _role;

        [ForeignKey("Role")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(Role))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. Role: " + Role + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(Role);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    Role = _role == null ? default : _role.RoleId;
                }
            }
        }

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

        [InverseProperty("Employee")]
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
                            throw new InvalidOperationException("Cannot access Projects - no database context is set. EmployeeId: " + this.EmployeeId + ".");
                        }
                        _projects = new ObservableCollection<Project>();
                    }
                    else
                    {
                        var items = Context.Projects.Where(x => x.ApprovedBy == this.EmployeeId).ToList<Project>();
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
                    item.ApprovedBy = this.EmployeeId;
                }
            }
        }

        private ObservableCollection<Role> _roles;

        [InverseProperty("Employee")]
        public virtual ObservableCollection<Role> Roles
        {
            get
            {
                if (_roles == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Roles - no database context is set. EmployeeId: " + this.EmployeeId + ".");
                        }
                        _roles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = Context.Roles.Where(x => x.Employees == this.EmployeeId).ToList<Role>();
                        _roles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
                return _roles;
            }
            private set
            {
                if (_roles != null)
                {
                    _roles.CollectionChanged -= Roles_CollectionChanged;
                }
                _roles = value;
                if (_roles != null)
                {
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
            }
        }

        private void Roles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.Employees = this.EmployeeId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.Project;
            _ = this.Projects;
            _ = this.Roles;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
