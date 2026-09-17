
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
    [Table("ProcessStages")]
    public class ProcessStageBase : SoAEntityBase
    {
        [Key]
        public string ProcessStageId { get; set; }

        // Formula Name (rulebook: ={{ProcedureVersion}} & " stage " & {{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProcedureVersion)), F.S(" stage "), F.Text(F.Of(this.Label))))); set { }
        }

        public string? Label { get; set; }
        public int? Sequence { get; set; }
        // Formula StepCount (rulebook: =COUNTIFS(Steps!{{Stage}}, {{ProcessStageId}}))
        [NotMapped]
        public int? StepCount
        {
            get => F.AsInt(F.Memo(this, "StepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.Stage), F.Of(this.ProcessStageId))))))); set { }
        }

        // Formula OwnerHasNoHolder (rulebook: =INDEX(Roles!{{HasNoCurrentHolder}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public bool? OwnerHasNoHolder
        {
            get => F.AsBool(F.Memo(this, "OwnerHasNoHolder", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.OwnerRole), __r => F.Of(__r.HasNoCurrentHolder), () => F.Of(new Role().HasNoCurrentHolder)))); set { }
        }

        // Formula IsUnownedOrEmptyStage (rulebook: =OR({{OwnerRole}} = "", {{OwnerHasNoHolder}}, {{StepCount}} = 0))
        [NotMapped]
        public bool? IsUnownedOrEmptyStage
        {
            get => F.AsBool(F.Memo(this, "IsUnownedOrEmptyStage", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.OwnerRole))), F.Bool3(F.Of(this.OwnerHasNoHolder)), F.Bool3(F.Eq(F.Of(this.StepCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? OwnerRole { get; set; }

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

        private Role _role;

        [ForeignKey("OwnerRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(OwnerRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. OwnerRole: " + OwnerRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(OwnerRole);
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
                        OwnerRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<Step> _steps;

        [InverseProperty("ProcessStage")]
        public virtual ObservableCollection<Step> Steps
        {
            get
            {
                if (_steps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Steps - no database context is set. ProcessStageId: " + this.ProcessStageId + ".");
                        }
                        _steps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = base.SoAContext.Steps.Where(x => x.Stage == this.ProcessStageId).ToList<Step>();
                        _steps = new ObservableCollection<Step>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _steps.CollectionChanged += Steps_CollectionChanged;
                }
                return _steps;
            }
            private set
            {
                if (_steps != null)
                {
                    _steps.CollectionChanged -= Steps_CollectionChanged;
                }
                _steps = value;
                if (_steps != null)
                {
                    _steps.CollectionChanged += Steps_CollectionChanged;
                }
            }
        }

        private void Steps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Step>())
                {
                    item.Stage = this.ProcessStageId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.Role;
            _ = this.Steps;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
