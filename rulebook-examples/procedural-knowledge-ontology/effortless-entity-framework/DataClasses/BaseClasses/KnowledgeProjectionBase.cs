
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
    [Table("KnowledgeProjections")]
    public class KnowledgeProjectionBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeProjectionId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? ProjectionKind { get; set; }
        public string? Notation { get; set; }
        public string? OutputPath { get; set; }
        public string? GeneratedByTool { get; set; }
        public DateTimeOffset? GeneratedAt { get; set; }
        public string? PublishedUri { get; set; }
        // Formula VersionModifiedAt (rulebook: =INDEX(ProcedureVersions!{{ModifiedAt}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public DateTimeOffset? VersionModifiedAt
        {
            get => F.AsDateTime(F.Memo(this, "VersionModifiedAt", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.ModifiedAt), () => F.Of(new ProcedureVersion().ModifiedAt)))); set { }
        }

        // Formula OpenCount (rulebook: =COUNTIFS(KnowledgeSearchEvents!{{OpenedProjection}}, {{KnowledgeProjectionId}}))
        [NotMapped]
        public int? OpenCount
        {
            get => F.AsInt(F.Memo(this, "OpenCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeSearchEvent>(base.SoAContext, "KnowledgeSearchEvents", __c => __c.KnowledgeSearchEvents), __r => F.CritField(F.Of(__r.OpenedProjection), F.Of(this.KnowledgeProjectionId))))))); set { }
        }

        // Formula DiagramCanDivergeFromModel (rulebook: =AND({{ProjectionKind}} = "Diagram", OR({{GeneratedByTool}} = "", {{GeneratedAt}} < {{VersionModifiedAt}})))
        [NotMapped]
        public bool? DiagramCanDivergeFromModel
        {
            get => F.AsBool(F.Memo(this, "DiagramCanDivergeFromModel", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ProjectionKind)), F.S("Diagram"))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.GeneratedByTool))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.GeneratedAt)), "<", F.Of(this.VersionModifiedAt)))))))); set { }
        }

        // Formula NarrativeNotGeneratedFromModel (rulebook: =AND({{ProjectionKind}} = "Narrative", OR({{GeneratedByTool}} = "", {{GeneratedAt}} < {{VersionModifiedAt}})))
        [NotMapped]
        public bool? NarrativeNotGeneratedFromModel
        {
            get => F.AsBool(F.Memo(this, "NarrativeNotGeneratedFromModel", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ProjectionKind)), F.S("Narrative"))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.GeneratedByTool))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.GeneratedAt)), "<", F.Of(this.VersionModifiedAt)))))))); set { }
        }

        // Formula NarrativeUnreachable (rulebook: =AND({{ProjectionKind}} = "Narrative", {{PublishedUri}} = "", {{OpenCount}} = 0))
        [NotMapped]
        public bool? NarrativeUnreachable
        {
            get => F.AsBool(F.Memo(this, "NarrativeUnreachable", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ProjectionKind)), F.S("Narrative"))), F.Bool3(F.IsBlank(F.Of(this.PublishedUri))), F.Bool3(F.Eq(F.Of(this.OpenCount), F.I(0)))))); set { }
        }

        // Formula IsPublished (rulebook: ={{PublishedUri}} <> "")
        [NotMapped]
        public bool? IsPublished
        {
            get => F.AsBool(F.Memo(this, "IsPublished", () => F.IsNotBlank(F.Of(this.PublishedUri)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? AudienceRole { get; set; }

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

        [ForeignKey("AudienceRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(AudienceRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AudienceRole: " + AudienceRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(AudienceRole);
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
                        AudienceRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<KnowledgeSearchEvent> _knowledgeSearchEvents;

        [InverseProperty("KnowledgeProjection")]
        public virtual ObservableCollection<KnowledgeSearchEvent> KnowledgeSearchEvents
        {
            get
            {
                if (_knowledgeSearchEvents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeSearchEvents - no database context is set. KnowledgeProjectionId: " + this.KnowledgeProjectionId + ".");
                        }
                        _knowledgeSearchEvents = new ObservableCollection<KnowledgeSearchEvent>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeSearchEvents.Where(x => x.OpenedProjection == this.KnowledgeProjectionId).ToList<KnowledgeSearchEvent>();
                        _knowledgeSearchEvents = new ObservableCollection<KnowledgeSearchEvent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeSearchEvents.CollectionChanged += KnowledgeSearchEvents_CollectionChanged;
                }
                return _knowledgeSearchEvents;
            }
            private set
            {
                if (_knowledgeSearchEvents != null)
                {
                    _knowledgeSearchEvents.CollectionChanged -= KnowledgeSearchEvents_CollectionChanged;
                }
                _knowledgeSearchEvents = value;
                if (_knowledgeSearchEvents != null)
                {
                    _knowledgeSearchEvents.CollectionChanged += KnowledgeSearchEvents_CollectionChanged;
                }
            }
        }

        private void KnowledgeSearchEvents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeSearchEvent>())
                {
                    item.OpenedProjection = this.KnowledgeProjectionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.Role;
            _ = this.KnowledgeSearchEvents;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
