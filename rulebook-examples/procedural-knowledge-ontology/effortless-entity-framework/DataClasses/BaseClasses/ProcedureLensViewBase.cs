
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
    [Table("ProcedureLensViews")]
    public class ProcedureLensViewBase : SoAEntityBase
    {
        [Key]
        public string ProcedureLensViewId { get; set; }

        // Formula Name (rulebook: ={{Procedure}} & " for " & {{StakeholderLens}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Procedure)), F.S(" for "), F.Text(F.Of(this.StakeholderLens))))); set { }
        }

        public string? Granularity { get; set; }
        public string? Form { get; set; }
        // Formula ProcedureCurrentVersion (rulebook: =INDEX(Procedures!{{CurrentVersionKey}}, MATCH({{Procedure}}, Procedures!{{ProcedureId}}, 0)))
        [NotMapped]
        public string? ProcedureCurrentVersion
        {
            get => F.AsString(F.Memo(this, "ProcedureCurrentVersion", () => F.Lookup<Procedure>(this, "Procedures", "ProcedureId", __c => __c.Procedures, __r => F.Of(__r.ProcedureId), F.Of(this.Procedure), __r => F.Of(__r.CurrentVersionKey), () => F.Of(new Procedure().CurrentVersionKey)))); set { }
        }

        // Formula LensGranularityRank (rulebook: =INDEX(StakeholderLenses!{{GranularityRank}}, MATCH({{StakeholderLens}}, StakeholderLenses!{{StakeholderLensId}}, 0)))
        [NotMapped]
        public int? LensGranularityRank
        {
            get => F.AsInt(F.Memo(this, "LensGranularityRank", () => F.Integer(F.Lookup<StakeholderLense>(this, "StakeholderLenses", "StakeholderLensId", __c => __c.StakeholderLenses, __r => F.Of(__r.StakeholderLensId), F.Of(this.StakeholderLens), __r => F.Of(__r.GranularityRank), () => F.Of(new StakeholderLense().GranularityRank))))); set { }
        }

        // Formula ViewGranularityRank (rulebook: =IF({{Granularity}} = "Step", 3, IF({{Granularity}} = "Stage", 2, IF({{Granularity}} = "Category", 1, 0))))
        [NotMapped]
        public int? ViewGranularityRank
        {
            get => F.AsInt(F.Memo(this, "ViewGranularityRank", () => F.Integer((F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Granularity)), F.S("Step")))) ? F.I(3) : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Granularity)), F.S("Stage")))) ? F.I(2) : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Granularity)), F.S("Category")))) ? F.I(1) : F.I(0))))))); set { }
        }

        // Formula IsGranularityMisfit (rulebook: =AND({{LensGranularityRank}} > 0, {{ViewGranularityRank}} <> {{LensGranularityRank}}))
        [NotMapped]
        public bool? IsGranularityMisfit
        {
            get => F.AsBool(F.Memo(this, "IsGranularityMisfit", () => F.And(F.Bool3(F.Cmp(F.Of(this.LensGranularityRank), ">", F.I(0))), F.Bool3(F.Ne(F.Of(this.ViewGranularityRank), F.Of(this.LensGranularityRank)))))); set { }
        }

        // Formula IsDisconnectedSilo (rulebook: =OR({{ProjectsVersion}} = "", {{ProjectsVersion}} <> {{ProcedureCurrentVersion}}))
        [NotMapped]
        public bool? IsDisconnectedSilo
        {
            get => F.AsBool(F.Memo(this, "IsDisconnectedSilo", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.ProjectsVersion))), F.Bool3(F.Ne(F.Nullif(F.Of(this.ProjectsVersion)), F.Of(this.ProcedureCurrentVersion)))))); set { }
        }

        // Formula StepLevelProcedureKey (rulebook: =IF({{LensGranularityRank}} = 3, {{Procedure}}, ""))
        [NotMapped]
        public string? StepLevelProcedureKey
        {
            get => F.AsString(F.Memo(this, "StepLevelProcedureKey", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.LensGranularityRank), F.I(3)))) ? F.Of(this.Procedure) : F.S("")))); set { }
        }

        // Formula CategoryLevelProcedureKey (rulebook: =IF({{LensGranularityRank}} = 1, {{Procedure}}, ""))
        [NotMapped]
        public string? CategoryLevelProcedureKey
        {
            get => F.AsString(F.Memo(this, "CategoryLevelProcedureKey", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.LensGranularityRank), F.I(1)))) ? F.Of(this.Procedure) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
        public string? StakeholderLens { get; set; }
        public string? ProjectsVersion { get; set; }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }

        private StakeholderLense _stakeholderLense;

        [ForeignKey("StakeholderLens")]
        public virtual StakeholderLense StakeholderLense
        {
            get
            {
                if (_stakeholderLense == null && !string.IsNullOrEmpty(StakeholderLens))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StakeholderLense - no database context is set. StakeholderLens: " + StakeholderLens + ".");
                        }
                        return null;
                    }
                    _stakeholderLense = base.SoAContext.StakeholderLenses.Find(StakeholderLens);
                    if (_stakeholderLense != null)
                    {
                        base.SoAContext.Attach(_stakeholderLense);
                    }
                }
                return _stakeholderLense;
            }
            set
            {
                if (_stakeholderLense != value)
                {
                    _stakeholderLense = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stakeholderLense != null)
                    {
                        StakeholderLens = _stakeholderLense.StakeholderLensId;
                    }
                }
            }
        }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("ProjectsVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(ProjectsVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. ProjectsVersion: " + ProjectsVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = base.SoAContext.ProcedureVersions.Find(ProjectsVersion);
                    if (_procedureVersion != null)
                    {
                        base.SoAContext.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersion != null)
                    {
                        ProjectsVersion = _procedureVersion.ProcedureVersionId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.StakeholderLense;
            _ = this.ProcedureVersion;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
