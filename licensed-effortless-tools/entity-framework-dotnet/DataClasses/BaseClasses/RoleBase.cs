
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Roles")]
    public class RoleBase : SoAEntityBase
    {
        [Key]
        public string RoleId { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsManager { get; set; }
        // Formula CountOfEmployees (rulebook: =COUNTIFS(Employees!{{Role}}, Roles!{{RoleId}}))
        public int? CountOfEmployees
        {
            get => this.Employees == null ? 0 : this.Employees.Count; set { }
        }


        public string? Employees { get; set; }

        private Employee _employee;

        [ForeignKey("Employees")]
        public virtual Employee Employee
        {
            get
            {
                if (_employee == null && !string.IsNullOrEmpty(Employees))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Employee - no database context is set. Employees: " + Employees + ".");
                        }
                        return null;
                    }
                    _employee = Context.Employees.Find(Employees);
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
                    Employees = _employee == null ? default : _employee.EmployeeId;
                }
            }
        }

        private ObservableCollection<Employee> _employees;

        [InverseProperty("Role")]
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
                            throw new InvalidOperationException("Cannot access Employees - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _employees = new ObservableCollection<Employee>();
                    }
                    else
                    {
                        var items = Context.Employees.Where(x => x.Role == this.RoleId).ToList<Employee>();
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
                    item.Role = this.RoleId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Employee;
            _ = this.Employees;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
