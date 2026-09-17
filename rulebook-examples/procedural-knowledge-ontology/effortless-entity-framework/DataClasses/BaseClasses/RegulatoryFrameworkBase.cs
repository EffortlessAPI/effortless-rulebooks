
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
    [Table("RegulatoryFrameworks")]
    public class RegulatoryFrameworkBase : SoAEntityBase
    {
        [Key]
        public string RegulatoryFrameworkId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Jurisdiction { get; set; }
        // Formula RequirementCount (rulebook: =COUNTIFS(Requirements!{{RegulatoryFramework}}, {{RegulatoryFrameworkId}}))
        [NotMapped]
        public int? RequirementCount
        {
            get => F.AsInt(F.Memo(this, "RequirementCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Requirement>(base.SoAContext, "Requirements", __c => __c.Requirements), __r => F.CritField(F.Of(__r.RegulatoryFramework), F.Of(this.RegulatoryFrameworkId))))))); set { }
        }

        // Formula RequiredProcedureCount (rulebook: =COUNTIFS(Procedures!{{RequiredByRegulation}}, {{RegulatoryFrameworkId}}))
        [NotMapped]
        public int? RequiredProcedureCount
        {
            get => F.AsInt(F.Memo(this, "RequiredProcedureCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Procedure>(base.SoAContext, "Procedures", __c => __c.Procedures), __r => F.CritField(F.Of(__r.RequiredByRegulation), F.Of(this.RegulatoryFrameworkId))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<Procedure> _procedures;

        [InverseProperty("RegulatoryFramework")]
        public virtual ObservableCollection<Procedure> Procedures
        {
            get
            {
                if (_procedures == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedures - no database context is set. RegulatoryFrameworkId: " + this.RegulatoryFrameworkId + ".");
                        }
                        _procedures = new ObservableCollection<Procedure>();
                    }
                    else
                    {
                        var items = base.SoAContext.Procedures.Where(x => x.RequiredByRegulation == this.RegulatoryFrameworkId).ToList<Procedure>();
                        _procedures = new ObservableCollection<Procedure>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
                return _procedures;
            }
            private set
            {
                if (_procedures != null)
                {
                    _procedures.CollectionChanged -= Procedures_CollectionChanged;
                }
                _procedures = value;
                if (_procedures != null)
                {
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
            }
        }

        private void Procedures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Procedure>())
                {
                    item.RequiredByRegulation = this.RegulatoryFrameworkId;
                }
            }
        }

        private ObservableCollection<Requirement> _requirements;

        [InverseProperty("RegulatoryFrameworkRef")]
        public virtual ObservableCollection<Requirement> Requirements
        {
            get
            {
                if (_requirements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Requirements - no database context is set. RegulatoryFrameworkId: " + this.RegulatoryFrameworkId + ".");
                        }
                        _requirements = new ObservableCollection<Requirement>();
                    }
                    else
                    {
                        var items = base.SoAContext.Requirements.Where(x => x.RegulatoryFramework == this.RegulatoryFrameworkId).ToList<Requirement>();
                        _requirements = new ObservableCollection<Requirement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _requirements.CollectionChanged += Requirements_CollectionChanged;
                }
                return _requirements;
            }
            private set
            {
                if (_requirements != null)
                {
                    _requirements.CollectionChanged -= Requirements_CollectionChanged;
                }
                _requirements = value;
                if (_requirements != null)
                {
                    _requirements.CollectionChanged += Requirements_CollectionChanged;
                }
            }
        }

        private void Requirements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Requirement>())
                {
                    item.RegulatoryFramework = this.RegulatoryFrameworkId;
                }
            }
        }

        private ObservableCollection<ApplicabilityScope> _applicabilityScopes;

        [InverseProperty("RegulatoryFramework")]
        public virtual ObservableCollection<ApplicabilityScope> ApplicabilityScopes
        {
            get
            {
                if (_applicabilityScopes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApplicabilityScopes - no database context is set. RegulatoryFrameworkId: " + this.RegulatoryFrameworkId + ".");
                        }
                        _applicabilityScopes = new ObservableCollection<ApplicabilityScope>();
                    }
                    else
                    {
                        var items = base.SoAContext.ApplicabilityScopes.Where(x => x.RegulatoryRegime == this.RegulatoryFrameworkId).ToList<ApplicabilityScope>();
                        _applicabilityScopes = new ObservableCollection<ApplicabilityScope>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _applicabilityScopes.CollectionChanged += ApplicabilityScopes_CollectionChanged;
                }
                return _applicabilityScopes;
            }
            private set
            {
                if (_applicabilityScopes != null)
                {
                    _applicabilityScopes.CollectionChanged -= ApplicabilityScopes_CollectionChanged;
                }
                _applicabilityScopes = value;
                if (_applicabilityScopes != null)
                {
                    _applicabilityScopes.CollectionChanged += ApplicabilityScopes_CollectionChanged;
                }
            }
        }

        private void ApplicabilityScopes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ApplicabilityScope>())
                {
                    item.RegulatoryRegime = this.RegulatoryFrameworkId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Procedures;
            _ = this.Requirements;
            _ = this.ApplicabilityScopes;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
