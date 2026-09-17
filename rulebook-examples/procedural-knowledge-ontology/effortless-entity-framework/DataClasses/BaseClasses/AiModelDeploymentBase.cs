
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
    [Table("AiModelDeployments")]
    public class AiModelDeploymentBase : SoAEntityBase
    {
        [Key]
        public string AiModelDeploymentId { get; set; }

        // Formula Name (rulebook: ={{ModelVersion}} & " @ " & {{Environment}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ModelVersion)), F.S(" @ "), F.Text(F.Of(this.Environment))))); set { }
        }

        public string? Environment { get; set; }
        public DateTimeOffset? DeployedAt { get; set; }
        public DateTimeOffset? RetiredAt { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula IsLiveInProduction (rulebook: =AND({{Environment}} = "Production", {{DeployedAt}} <= {{AsOfInstant}}, OR({{RetiredAt}} = "", {{RetiredAt}} > {{AsOfInstant}})))
        [NotMapped]
        public bool? IsLiveInProduction
        {
            get => F.AsBool(F.Memo(this, "IsLiveInProduction", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Environment)), F.S("Production"))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.DeployedAt)), "<=", F.Of(this.AsOfInstant))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.RetiredAt))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.RetiredAt)), ">", F.Of(this.AsOfInstant)))))))); set { }
        }

        // Formula AgentIdentifier (rulebook: =INDEX(AiRegistryModelVersions!{{DcIdentifier}}, MATCH({{ModelVersion}}, AiRegistryModelVersions!{{AiRegistryModelVersionId}}, 0)))
        [NotMapped]
        public string? AgentIdentifier
        {
            get => F.AsString(F.Memo(this, "AgentIdentifier", () => F.Lookup<AiRegistryModelVersion>(this, "AiRegistryModelVersions", "AiRegistryModelVersionId", __c => __c.AiRegistryModelVersions, __r => F.Of(__r.AiRegistryModelVersionId), F.Of(this.ModelVersion), __r => F.Of(__r.DcIdentifier), () => F.Of(new AiRegistryModelVersion().DcIdentifier)))); set { }
        }

        // Formula AssignmentLinkCount (rulebook: =COUNTIFS(RoleAssignments!{{Agent}}, {{AgentIdentifier}}))
        [NotMapped]
        public int? AssignmentLinkCount
        {
            get => F.AsInt(F.Memo(this, "AssignmentLinkCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.Agent), F.Of(this.AgentIdentifier))))))); set { }
        }

        // Formula ArtifactLinkCount (rulebook: =COUNTIFS(ExecutionEntities!{{AttributedToAgent}}, {{AgentIdentifier}}))
        [NotMapped]
        public int? ArtifactLinkCount
        {
            get => F.AsInt(F.Memo(this, "ArtifactLinkCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ExecutionEntity>(base.SoAContext, "ExecutionEntities", __c => __c.ExecutionEntities), __r => F.CritField(F.Of(__r.AttributedToAgent), F.Of(this.AgentIdentifier))))))); set { }
        }

        // Formula IsIsolatedRegistryFact (rulebook: =AND({{IsLiveInProduction}}, {{AssignmentLinkCount}} = 0, {{ArtifactLinkCount}} = 0))
        [NotMapped]
        public bool? IsIsolatedRegistryFact
        {
            get => F.AsBool(F.Memo(this, "IsIsolatedRegistryFact", () => F.And(F.Bool3(F.Of(this.IsLiveInProduction)), F.Bool3(F.Eq(F.Of(this.AssignmentLinkCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.ArtifactLinkCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ModelVersion { get; set; }
        public string? EvaluationContext { get; set; }

        private AiRegistryModelVersion _aiRegistryModelVersion;

        [ForeignKey("ModelVersion")]
        public virtual AiRegistryModelVersion AiRegistryModelVersion
        {
            get
            {
                if (_aiRegistryModelVersion == null && !string.IsNullOrEmpty(ModelVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiRegistryModelVersion - no database context is set. ModelVersion: " + ModelVersion + ".");
                        }
                        return null;
                    }
                    _aiRegistryModelVersion = base.SoAContext.AiRegistryModelVersions.Find(ModelVersion);
                    if (_aiRegistryModelVersion != null)
                    {
                        base.SoAContext.Attach(_aiRegistryModelVersion);
                    }
                }
                return _aiRegistryModelVersion;
            }
            set
            {
                if (_aiRegistryModelVersion != value)
                {
                    _aiRegistryModelVersion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_aiRegistryModelVersion != null)
                    {
                        ModelVersion = _aiRegistryModelVersion.AiRegistryModelVersionId;
                    }
                }
            }
        }

        private EvaluationContext _evaluationContextRef;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContextRef
        {
            get
            {
                if (_evaluationContextRef == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContextRef - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContextRef = base.SoAContext.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContextRef != null)
                    {
                        base.SoAContext.Attach(_evaluationContextRef);
                    }
                }
                return _evaluationContextRef;
            }
            set
            {
                if (_evaluationContextRef != value)
                {
                    _evaluationContextRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_evaluationContextRef != null)
                    {
                        EvaluationContext = _evaluationContextRef.EvaluationContextId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AiRegistryModelVersion;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
