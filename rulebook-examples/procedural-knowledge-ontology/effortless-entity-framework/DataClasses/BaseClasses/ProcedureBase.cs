
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Procedures")]
    public class ProcedureBase : SoAEntityBase
    {
        [Key]
        public string ProcedureId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        public string? Name
        {
            get => this.Title; set { }
        }

        public string? Title { get; set; }
        public string? Purpose { get; set; }
        public string? Target { get; set; }
        public bool? IsTemplate { get; set; }
        public string? CurrentVersionKey { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? ProcedureType { get; set; }
        public string? OwnerOrganization { get; set; }
        public string? AdoptedByOrganization { get; set; }

        private ProcedureType _procedureType;

        [ForeignKey("ProcedureType")]
        public virtual ProcedureType ProcedureType
        {
            get
            {
                if (_procedureType == null && !string.IsNullOrEmpty(ProcedureType))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureType - no database context is set. ProcedureType: " + ProcedureType + ".");
                        }
                        return null;
                    }
                    _procedureType = Context.ProcedureTypes.Find(ProcedureType);
                    if (_procedureType != null)
                    {
                        Context.Attach(_procedureType);
                    }
                }
                return _procedureType;
            }
            set
            {
                if (_procedureType != value)
                {
                    _procedureType = value;
                    ProcedureType = _procedureType == null ? default : _procedureType.ProcedureTypeId;
                }
            }
        }

        private Organization _organization;

        [ForeignKey("OwnerOrganization")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(OwnerOrganization))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. OwnerOrganization: " + OwnerOrganization + ".");
                        }
                        return null;
                    }
                    _organization = Context.Organizations.Find(OwnerOrganization);
                    if (_organization != null)
                    {
                        Context.Attach(_organization);
                    }
                }
                return _organization;
            }
            set
            {
                if (_organization != value)
                {
                    _organization = value;
                    OwnerOrganization = _organization == null ? default : _organization.OrganizationId;
                }
            }
        }

        private Organization _organization;

        [ForeignKey("AdoptedByOrganization")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(AdoptedByOrganization))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. AdoptedByOrganization: " + AdoptedByOrganization + ".");
                        }
                        return null;
                    }
                    _organization = Context.Organizations.Find(AdoptedByOrganization);
                    if (_organization != null)
                    {
                        Context.Attach(_organization);
                    }
                }
                return _organization;
            }
            set
            {
                if (_organization != value)
                {
                    _organization = value;
                    AdoptedByOrganization = _organization == null ? default : _organization.OrganizationId;
                }
            }
        }

        private ObservableCollection<ProcedureVersion> _procedureVersions;

        [InverseProperty("Procedure")]
        public virtual ObservableCollection<ProcedureVersion> ProcedureVersions
        {
            get
            {
                if (_procedureVersions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersions - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _procedureVersions = new ObservableCollection<ProcedureVersion>();
                    }
                    else
                    {
                        var items = Context.ProcedureVersions.Where(x => x.Procedure == this.ProcedureId).ToList<ProcedureVersion>();
                        _procedureVersions = new ObservableCollection<ProcedureVersion>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _procedureVersions.CollectionChanged += ProcedureVersions_CollectionChanged;
                }
                return _procedureVersions;
            }
            private set
            {
                if (_procedureVersions != null)
                {
                    _procedureVersions.CollectionChanged -= ProcedureVersions_CollectionChanged;
                }
                _procedureVersions = value;
                if (_procedureVersions != null)
                {
                    _procedureVersions.CollectionChanged += ProcedureVersions_CollectionChanged;
                }
            }
        }

        private void ProcedureVersions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureVersion>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureType;
            _ = this.Organization;
            _ = this.Organization;
            _ = this.ProcedureVersions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
