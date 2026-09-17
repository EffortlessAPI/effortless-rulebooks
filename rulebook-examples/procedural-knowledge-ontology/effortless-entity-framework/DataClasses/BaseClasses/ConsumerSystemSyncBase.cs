
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
    [Table("ConsumerSystemSyncs")]
    public class ConsumerSystemSyncBase : SoAEntityBase
    {
        [Key]
        public string ConsumerSystemSyncId { get; set; }

        // Formula Name (rulebook: ={{ConsumerSystem}} & " holds " & {{LoadedVersion}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ConsumerSystem)), F.S(" holds "), F.Text(F.Of(this.LoadedVersion))))); set { }
        }

        public DateTimeOffset? SyncedAt { get; set; }
        public bool? CarriesProvenance { get; set; }
        // Formula SystemAudience (rulebook: =INDEX(KnowledgeConsumerSystems!{{Audience}}, MATCH({{ConsumerSystem}}, KnowledgeConsumerSystems!{{KnowledgeConsumerSystemId}}, 0)))
        [NotMapped]
        public string? SystemAudience
        {
            get => F.AsString(F.Memo(this, "SystemAudience", () => F.Lookup<KnowledgeConsumerSystem>(this, "KnowledgeConsumerSystems", "KnowledgeConsumerSystemId", __c => __c.KnowledgeConsumerSystems, __r => F.Of(__r.KnowledgeConsumerSystemId), F.Of(this.ConsumerSystem), __r => F.Of(__r.Audience), () => F.Of(new KnowledgeConsumerSystem().Audience)))); set { }
        }

        // Formula LoadedProcedure (rulebook: =INDEX(ProcedureVersions!{{Procedure}}, MATCH({{LoadedVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public string? LoadedProcedure
        {
            get => F.AsString(F.Memo(this, "LoadedProcedure", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.LoadedVersion), __r => F.Of(__r.Procedure), () => F.Of(new ProcedureVersion().Procedure)))); set { }
        }

        // Formula CanonicalVersion (rulebook: =INDEX(Procedures!{{CurrentVersionKey}}, MATCH({{LoadedProcedure}}, Procedures!{{ProcedureId}}, 0)))
        [NotMapped]
        public string? CanonicalVersion
        {
            get => F.AsString(F.Memo(this, "CanonicalVersion", () => F.Lookup<Procedure>(this, "Procedures", "ProcedureId", __c => __c.Procedures, __r => F.Of(__r.ProcedureId), F.Of(this.LoadedProcedure), __r => F.Of(__r.CurrentVersionKey), () => F.Of(new Procedure().CurrentVersionKey)))); set { }
        }

        // Formula CanonicalVersionModifiedAt (rulebook: =INDEX(ProcedureVersions!{{ModifiedAt}}, MATCH({{CanonicalVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public DateTimeOffset? CanonicalVersionModifiedAt
        {
            get => F.AsDateTime(F.Memo(this, "CanonicalVersionModifiedAt", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.CanonicalVersion), __r => F.Of(__r.ModifiedAt), () => F.Of(new ProcedureVersion().ModifiedAt)))); set { }
        }

        // Formula LoadedVersionCreator (rulebook: =INDEX(ProcedureVersions!{{CreatedByAgent}}, MATCH({{LoadedVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public string? LoadedVersionCreator
        {
            get => F.AsString(F.Memo(this, "LoadedVersionCreator", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.LoadedVersion), __r => F.Of(__r.CreatedByAgent), () => F.Of(new ProcedureVersion().CreatedByAgent)))); set { }
        }

        // Formula IsBehindCanonicalVersion (rulebook: =AND({{CanonicalVersion}} <> "", {{LoadedVersion}} <> {{CanonicalVersion}}))
        [NotMapped]
        public bool? IsBehindCanonicalVersion
        {
            get => F.AsBool(F.Memo(this, "IsBehindCanonicalVersion", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.CanonicalVersion))), F.Bool3(F.Ne(F.Nullif(F.Of(this.LoadedVersion)), F.Of(this.CanonicalVersion)))))); set { }
        }

        // Formula PredatesVersionChange (rulebook: =AND({{LoadedVersion}} = {{CanonicalVersion}}, {{SyncedAt}} < {{CanonicalVersionModifiedAt}}))
        [NotMapped]
        public bool? PredatesVersionChange
        {
            get => F.AsBool(F.Memo(this, "PredatesVersionChange", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.LoadedVersion)), F.Of(this.CanonicalVersion))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.SyncedAt)), "<", F.Of(this.CanonicalVersionModifiedAt)))))); set { }
        }

        // Formula IsAiFedFromForkedCopy (rulebook: =AND({{SystemAudience}} <> "Human", {{SourceResource}} <> ""))
        [NotMapped]
        public bool? IsAiFedFromForkedCopy
        {
            get => F.AsBool(F.Memo(this, "IsAiFedFromForkedCopy", () => F.And(F.Bool3(F.Ne(F.Of(this.SystemAudience), F.S("Human"))), F.Bool3(F.IsNotBlank(F.Of(this.SourceResource)))))); set { }
        }

        // Formula DropsProvenanceInTransit (rulebook: =AND({{LoadedVersionCreator}} <> "", {{CarriesProvenance}} = FALSE))
        [NotMapped]
        public bool? DropsProvenanceInTransit
        {
            get => F.AsBool(F.Memo(this, "DropsProvenanceInTransit", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.LoadedVersionCreator))), F.Bool3(F.Eq(F.Nullif(F.Of(this.CarriesProvenance)), F.B(false)))))); set { }
        }

        // Formula ReachesHumans (rulebook: =OR({{SystemAudience}} = "Human", {{SystemAudience}} = "Both"))
        [NotMapped]
        public bool? ReachesHumans
        {
            get => F.AsBool(F.Memo(this, "ReachesHumans", () => F.Or(F.Bool3(F.Eq(F.Of(this.SystemAudience), F.S("Human"))), F.Bool3(F.Eq(F.Of(this.SystemAudience), F.S("Both")))))); set { }
        }

        // Formula ReachesMachines (rulebook: =OR({{SystemAudience}} = "AI", {{SystemAudience}} = "Both"))
        [NotMapped]
        public bool? ReachesMachines
        {
            get => F.AsBool(F.Memo(this, "ReachesMachines", () => F.Or(F.Bool3(F.Eq(F.Of(this.SystemAudience), F.S("AI"))), F.Bool3(F.Eq(F.Of(this.SystemAudience), F.S("Both")))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ConsumerSystem { get; set; }
        public string? LoadedVersion { get; set; }
        public string? SourceResource { get; set; }

        private KnowledgeConsumerSystem _knowledgeConsumerSystem;

        [ForeignKey("ConsumerSystem")]
        public virtual KnowledgeConsumerSystem KnowledgeConsumerSystem
        {
            get
            {
                if (_knowledgeConsumerSystem == null && !string.IsNullOrEmpty(ConsumerSystem))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeConsumerSystem - no database context is set. ConsumerSystem: " + ConsumerSystem + ".");
                        }
                        return null;
                    }
                    _knowledgeConsumerSystem = base.SoAContext.KnowledgeConsumerSystems.Find(ConsumerSystem);
                    if (_knowledgeConsumerSystem != null)
                    {
                        base.SoAContext.Attach(_knowledgeConsumerSystem);
                    }
                }
                return _knowledgeConsumerSystem;
            }
            set
            {
                if (_knowledgeConsumerSystem != value)
                {
                    _knowledgeConsumerSystem = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeConsumerSystem != null)
                    {
                        ConsumerSystem = _knowledgeConsumerSystem.KnowledgeConsumerSystemId;
                    }
                }
            }
        }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("LoadedVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(LoadedVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. LoadedVersion: " + LoadedVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = base.SoAContext.ProcedureVersions.Find(LoadedVersion);
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
                        LoadedVersion = _procedureVersion.ProcedureVersionId;
                    }
                }
            }
        }

        private Resource _resource;

        [ForeignKey("SourceResource")]
        public virtual Resource Resource
        {
            get
            {
                if (_resource == null && !string.IsNullOrEmpty(SourceResource))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. SourceResource: " + SourceResource + ".");
                        }
                        return null;
                    }
                    _resource = base.SoAContext.Resources.Find(SourceResource);
                    if (_resource != null)
                    {
                        base.SoAContext.Attach(_resource);
                    }
                }
                return _resource;
            }
            set
            {
                if (_resource != value)
                {
                    _resource = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_resource != null)
                    {
                        SourceResource = _resource.ResourceId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.KnowledgeConsumerSystem;
            _ = this.ProcedureVersion;
            _ = this.Resource;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
