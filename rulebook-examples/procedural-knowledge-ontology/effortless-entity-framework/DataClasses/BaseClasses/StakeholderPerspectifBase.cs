
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
    [Table("StakeholderPerspectives")]
    public class StakeholderPerspectifBase : SoAEntityBase
    {
        [Key]
        public string StakeholderPerspectiveId { get; set; }

        // Formula Name (rulebook: ={{HolderRole}} & " on " & {{Step}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.HolderRole)), F.S(" on "), F.Text(F.Of(this.Step))))); set { }
        }

        public string? Position { get; set; }
        public string? Disposition { get; set; }
        // Formula SourceMaterialKind (rulebook: =INDEX(CollectedSourceMaterials!{{MaterialKind}}, MATCH({{SourceMaterial}}, CollectedSourceMaterials!{{CollectedSourceMaterialId}}, 0)))
        [NotMapped]
        public string? SourceMaterialKind
        {
            get => F.AsString(F.Memo(this, "SourceMaterialKind", () => F.Lookup<CollectedSourceMaterial>(this, "CollectedSourceMaterials", "CollectedSourceMaterialId", __c => __c.CollectedSourceMaterials, __r => F.Of(__r.CollectedSourceMaterialId), F.Of(this.SourceMaterial), __r => F.Of(__r.MaterialKind), () => F.Of(new CollectedSourceMaterial().MaterialKind)))); set { }
        }

        // Formula ConflictPartnerCount (rulebook: =COUNTIFS(StakeholderPerspectives!{{ConflictsWithPerspective}}, {{StakeholderPerspectiveId}}))
        [NotMapped]
        public int? ConflictPartnerCount
        {
            get => F.AsInt(F.Memo(this, "ConflictPartnerCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StakeholderPerspectif>(base.SoAContext, "StakeholderPerspectives", __c => __c.StakeholderPerspectives), __r => F.CritField(F.Of(__r.ConflictsWithPerspective), F.Of(this.StakeholderPerspectiveId))))))); set { }
        }

        // Formula IsInConflict (rulebook: =OR({{ConflictsWithPerspective}} <> "", {{ConflictPartnerCount}} > 0))
        [NotMapped]
        public bool? IsInConflict
        {
            get => F.AsBool(F.Memo(this, "IsInConflict", () => F.Or(F.Bool3(F.IsNotBlank(F.Of(this.ConflictsWithPerspective))), F.Bool3(F.Cmp(F.Of(this.ConflictPartnerCount), ">", F.I(0)))))); set { }
        }

        // Formula IsDissentingViewNotKeptWithSource (rulebook: =AND({{IsInConflict}}, OR({{Disposition}} = "OverriddenByConsensus", {{SourceMaterial}} = "")))
        [NotMapped]
        public bool? IsDissentingViewNotKeptWithSource
        {
            get => F.AsBool(F.Memo(this, "IsDissentingViewNotKeptWithSource", () => F.And(F.Bool3(F.Of(this.IsInConflict)), F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Disposition)), F.S("OverriddenByConsensus"))), F.Bool3(F.IsBlank(F.Of(this.SourceMaterial)))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? HolderRole { get; set; }
        public string? SourceMaterial { get; set; }
        public string? ConflictsWithPerspective { get; set; }

        private ProcedureVersion _procedureVersionRef;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersionRef
        {
            get
            {
                if (_procedureVersionRef == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionRef - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersionRef = base.SoAContext.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersionRef != null)
                    {
                        base.SoAContext.Attach(_procedureVersionRef);
                    }
                }
                return _procedureVersionRef;
            }
            set
            {
                if (_procedureVersionRef != value)
                {
                    _procedureVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersionRef != null)
                    {
                        ProcedureVersion = _procedureVersionRef.ProcedureVersionId;
                    }
                }
            }
        }

        private Step _stepRef;

        [ForeignKey("Step")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(Step))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(Step);
                    if (_stepRef != null)
                    {
                        base.SoAContext.Attach(_stepRef);
                    }
                }
                return _stepRef;
            }
            set
            {
                if (_stepRef != value)
                {
                    _stepRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepRef != null)
                    {
                        Step = _stepRef.StepId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("HolderRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(HolderRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. HolderRole: " + HolderRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(HolderRole);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        HolderRole = _role.RoleId;
                    }
                }
            }
        }

        private CollectedSourceMaterial _collectedSourceMaterial;

        [ForeignKey("SourceMaterial")]
        public virtual CollectedSourceMaterial CollectedSourceMaterial
        {
            get
            {
                if (_collectedSourceMaterial == null && !string.IsNullOrEmpty(SourceMaterial))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CollectedSourceMaterial - no database context is set. SourceMaterial: " + SourceMaterial + ".");
                        }
                        return null;
                    }
                    _collectedSourceMaterial = base.SoAContext.CollectedSourceMaterials.Find(SourceMaterial);
                    if (_collectedSourceMaterial != null)
                    {
                        base.SoAContext.Attach(_collectedSourceMaterial);
                    }
                }
                return _collectedSourceMaterial;
            }
            set
            {
                if (_collectedSourceMaterial != value)
                {
                    _collectedSourceMaterial = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_collectedSourceMaterial != null)
                    {
                        SourceMaterial = _collectedSourceMaterial.CollectedSourceMaterialId;
                    }
                }
            }
        }

        private StakeholderPerspectif _stakeholderPerspectif;

        [ForeignKey("ConflictsWithPerspective")]
        public virtual StakeholderPerspectif StakeholderPerspectif
        {
            get
            {
                if (_stakeholderPerspectif == null && !string.IsNullOrEmpty(ConflictsWithPerspective))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StakeholderPerspectif - no database context is set. ConflictsWithPerspective: " + ConflictsWithPerspective + ".");
                        }
                        return null;
                    }
                    _stakeholderPerspectif = base.SoAContext.StakeholderPerspectives.Find(ConflictsWithPerspective);
                    if (_stakeholderPerspectif != null)
                    {
                        base.SoAContext.Attach(_stakeholderPerspectif);
                    }
                }
                return _stakeholderPerspectif;
            }
            set
            {
                if (_stakeholderPerspectif != value)
                {
                    _stakeholderPerspectif = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stakeholderPerspectif != null)
                    {
                        ConflictsWithPerspective = _stakeholderPerspectif.StakeholderPerspectiveId;
                    }
                }
            }
        }

        private ObservableCollection<StakeholderPerspectif> _stakeholderPerspectives;

        [InverseProperty("StakeholderPerspectif")]
        public virtual ObservableCollection<StakeholderPerspectif> StakeholderPerspectives
        {
            get
            {
                if (_stakeholderPerspectives == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StakeholderPerspectives - no database context is set. StakeholderPerspectiveId: " + this.StakeholderPerspectiveId + ".");
                        }
                        _stakeholderPerspectives = new ObservableCollection<StakeholderPerspectif>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderPerspectives.Where(x => x.ConflictsWithPerspective == this.StakeholderPerspectiveId).ToList<StakeholderPerspectif>();
                        _stakeholderPerspectives = new ObservableCollection<StakeholderPerspectif>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stakeholderPerspectives.CollectionChanged += StakeholderPerspectives_CollectionChanged;
                }
                return _stakeholderPerspectives;
            }
            private set
            {
                if (_stakeholderPerspectives != null)
                {
                    _stakeholderPerspectives.CollectionChanged -= StakeholderPerspectives_CollectionChanged;
                }
                _stakeholderPerspectives = value;
                if (_stakeholderPerspectives != null)
                {
                    _stakeholderPerspectives.CollectionChanged += StakeholderPerspectives_CollectionChanged;
                }
            }
        }

        private void StakeholderPerspectives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StakeholderPerspectif>())
                {
                    item.ConflictsWithPerspective = this.StakeholderPerspectiveId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.StepRef;
            _ = this.Role;
            _ = this.CollectedSourceMaterial;
            _ = this.StakeholderPerspectif;
            _ = this.StakeholderPerspectives;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
