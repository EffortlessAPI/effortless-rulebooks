
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Projects")]
    public class ProjectBase : SoAEntityBase
    {
        [Key]
        public string ProjectId { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        // Formula ProjectTypeName (rulebook: =INDEX(TypesOfProject!{{Name}}, MATCH(Projects!{{ProjectType}}, TypesOfProject!{{TypesOfProjectId}}, 0)))
        public string? ProjectTypeName
        {
            get => this.TypesOfProject == null ? default : this.TypesOfProject.Name; set { }
        }

        // Formula ProjectTypeDescription (rulebook: =INDEX(TypesOfProject!{{Description}}, MATCH(Projects!{{ProjectType}}, TypesOfProject!{{TypesOfProjectId}}, 0)))
        public string? ProjectTypeDescription
        {
            get => this.TypesOfProject == null ? default : this.TypesOfProject.Description; set { }
        }

        // Formula ProjectTypeRequiresManagerApproval (rulebook: =INDEX(TypesOfProject!{{RequiresManagerApproval}}, MATCH(Projects!{{ProjectType}}, TypesOfProject!{{TypesOfProjectId}}, 0)))
        public bool? ProjectTypeRequiresManagerApproval
        {
            get => this.TypesOfProject == null ? default : this.TypesOfProject.RequiresManagerApproval; set { }
        }

        public DateTime? DueDate { get; set; }
        public bool? IsApproved { get; set; }
        // Formula ApprovedByRoleIsManager (rulebook: =INDEX(Employees!{{RoleIsManager}}, MATCH(Projects!{{ApprovedBy}}, Employees!{{EmployeeId}}, 0)))
        public bool? ApprovedByRoleIsManager
        {
            get => this.Employee == null ? default : this.Employee.RoleIsManager; set { }
        }

        // Formula ApprovedByName (rulebook: =INDEX(Employees!{{Name}}, MATCH(Projects!{{ApprovedBy}}, Employees!{{EmployeeId}}, 0)))
        public string? ApprovedByName
        {
            get => this.Employee == null ? default : this.Employee.Name; set { }
        }

        // Formula ApprovedByEmailAddress (rulebook: =INDEX(Employees!{{EmailAddress}}, MATCH(Projects!{{ApprovedBy}}, Employees!{{EmployeeId}}, 0)))
        public string? ApprovedByEmailAddress
        {
            get => this.Employee == null ? default : this.Employee.EmailAddress; set { }
        }

        // Formula ApprovedByPhoneNumber (rulebook: =INDEX(Employees!{{PhoneNumber}}, MATCH(Projects!{{ApprovedBy}}, Employees!{{EmployeeId}}, 0)))
        public string? ApprovedByPhoneNumber
        {
            get => this.Employee == null ? default : this.Employee.PhoneNumber; set { }
        }


        public string? ProjectType { get; set; }
        public string? ApprovedBy { get; set; }

        private TypesOfProject _typesOfProject;

        [ForeignKey("ProjectType")]
        public virtual TypesOfProject TypesOfProject
        {
            get
            {
                if (_typesOfProject == null && !string.IsNullOrEmpty(ProjectType))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TypesOfProject - no database context is set. ProjectType: " + ProjectType + ".");
                        }
                        return null;
                    }
                    _typesOfProject = Context.TypesOfProject.Find(ProjectType);
                    if (_typesOfProject != null)
                    {
                        Context.Attach(_typesOfProject);
                    }
                }
                return _typesOfProject;
            }
            set
            {
                if (_typesOfProject != value)
                {
                    _typesOfProject = value;
                    ProjectType = _typesOfProject == null ? default : _typesOfProject.TypesOfProjectId;
                }
            }
        }

        private Employee _employee;

        [ForeignKey("ApprovedBy")]
        public virtual Employee Employee
        {
            get
            {
                if (_employee == null && !string.IsNullOrEmpty(ApprovedBy))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Employee - no database context is set. ApprovedBy: " + ApprovedBy + ".");
                        }
                        return null;
                    }
                    _employee = Context.Employees.Find(ApprovedBy);
                    if (_employee != null)
                    {
                        Context.Attach(_employee);
                    }
                }
                return _employee;
            }
            set
            {
                if (_employee != value)
                {
                    _employee = value;
                    ApprovedBy = _employee == null ? default : _employee.EmployeeId;
                }
            }
        }

        private ObservableCollection<Employee> _employees;

        [InverseProperty("Project")]
        public virtual ObservableCollection<Employee> Employees
        {
            get
            {
                if (_employees == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Employees - no database context is set. ProjectId: " + this.ProjectId + ".");
                        }
                        _employees = new ObservableCollection<Employee>();
                    }
                    else
                    {
                        var items = Context.Employees.Where(x => x.Projects == this.ProjectId).ToList<Employee>();
                        _employees = new ObservableCollection<Employee>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _employees.CollectionChanged += Employees_CollectionChanged;
                }
                return _employees;
            }
            private set
            {
                if (_employees != null)
                {
                    _employees.CollectionChanged -= Employees_CollectionChanged;
                }
                _employees = value;
                if (_employees != null)
                {
                    _employees.CollectionChanged += Employees_CollectionChanged;
                }
            }
        }

        private void Employees_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Employee>())
                {
                    item.Projects = this.ProjectId;
                }
            }
        }

        private ObservableCollection<TypesOfProject> _typesOfProject;

        [InverseProperty("Project")]
        public virtual ObservableCollection<TypesOfProject> TypesOfProject
        {
            get
            {
                if (_typesOfProject == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TypesOfProject - no database context is set. ProjectId: " + this.ProjectId + ".");
                        }
                        _typesOfProject = new ObservableCollection<TypesOfProject>();
                    }
                    else
                    {
                        var items = Context.TypesOfProject.Where(x => x.Projects == this.ProjectId).ToList<TypesOfProject>();
                        _typesOfProject = new ObservableCollection<TypesOfProject>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _typesOfProject.CollectionChanged += TypesOfProject_CollectionChanged;
                }
                return _typesOfProject;
            }
            private set
            {
                if (_typesOfProject != null)
                {
                    _typesOfProject.CollectionChanged -= TypesOfProject_CollectionChanged;
                }
                _typesOfProject = value;
                if (_typesOfProject != null)
                {
                    _typesOfProject.CollectionChanged += TypesOfProject_CollectionChanged;
                }
            }
        }

        private void TypesOfProject_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TypesOfProject>())
                {
                    item.Projects = this.ProjectId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.TypesOfProject;
            _ = this.Employee;
            _ = this.Employees;
            _ = this.TypesOfProject;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
