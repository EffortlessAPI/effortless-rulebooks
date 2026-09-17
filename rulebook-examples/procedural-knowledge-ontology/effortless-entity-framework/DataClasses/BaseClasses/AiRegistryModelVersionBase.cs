
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
    [Table("AiRegistryModelVersions")]
    public class AiRegistryModelVersionBase : SoAEntityBase
    {
        [Key]
        public string AiRegistryModelVersionId { get; set; }

        // Formula Name (rulebook: ={{DcTitle}} & " " & {{DcHasVersion}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.DcTitle)), F.S(" "), F.Text(F.Of(this.DcHasVersion))))); set { }
        }

        public string? RegistrySystem { get; set; }
        public string? DcIdentifier { get; set; }
        public string? DcTitle { get; set; }
        public string? DcCreator { get; set; }
        public DateTimeOffset? DcDate { get; set; }
        public string? DcHasVersion { get; set; }
        public string? DcDescription { get; set; }
        public DateTimeOffset? FeedReceivedAt { get; set; }
        // Formula GraphIndividualCount (rulebook: =COUNTIFS(Agents!{{AgentId}}, {{DcIdentifier}}))
        [NotMapped]
        public int? GraphIndividualCount
        {
            get => F.AsInt(F.Memo(this, "GraphIndividualCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Agent>(base.SoAContext, "Agents", __c => __c.Agents), __r => F.CritField(F.Of(__r.AgentId), F.Of(this.DcIdentifier))))))); set { }
        }

        // Formula LiveProductionDeploymentCount (rulebook: =COUNTIFS(AiModelDeployments!{{ModelVersion}}, {{AiRegistryModelVersionId}}, AiModelDeployments!{{IsLiveInProduction}}, TRUE))
        [NotMapped]
        public int? LiveProductionDeploymentCount
        {
            get => F.AsInt(F.Memo(this, "LiveProductionDeploymentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AiModelDeployment>(base.SoAContext, "AiModelDeployments", __c => __c.AiModelDeployments), __r => F.CritField(F.Of(__r.ModelVersion), F.Of(this.AiRegistryModelVersionId)) && F.CritLiteral(F.Of(__r.IsLiveInProduction), F.B(true))))))); set { }
        }

        // Formula IsLiveButUnregisteredInGraph (rulebook: =AND({{LiveProductionDeploymentCount}} > 0, {{GraphIndividualCount}} = 0))
        [NotMapped]
        public bool? IsLiveButUnregisteredInGraph
        {
            get => F.AsBool(F.Memo(this, "IsLiveButUnregisteredInGraph", () => F.And(F.Bool3(F.Cmp(F.Of(this.LiveProductionDeploymentCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.GraphIndividualCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<AiModelDeployment> _aiModelDeployments;

        [InverseProperty("AiRegistryModelVersion")]
        public virtual ObservableCollection<AiModelDeployment> AiModelDeployments
        {
            get
            {
                if (_aiModelDeployments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiModelDeployments - no database context is set. AiRegistryModelVersionId: " + this.AiRegistryModelVersionId + ".");
                        }
                        _aiModelDeployments = new ObservableCollection<AiModelDeployment>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiModelDeployments.Where(x => x.ModelVersion == this.AiRegistryModelVersionId).ToList<AiModelDeployment>();
                        _aiModelDeployments = new ObservableCollection<AiModelDeployment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiModelDeployments.CollectionChanged += AiModelDeployments_CollectionChanged;
                }
                return _aiModelDeployments;
            }
            private set
            {
                if (_aiModelDeployments != null)
                {
                    _aiModelDeployments.CollectionChanged -= AiModelDeployments_CollectionChanged;
                }
                _aiModelDeployments = value;
                if (_aiModelDeployments != null)
                {
                    _aiModelDeployments.CollectionChanged += AiModelDeployments_CollectionChanged;
                }
            }
        }

        private void AiModelDeployments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiModelDeployment>())
                {
                    item.ModelVersion = this.AiRegistryModelVersionId;
                }
            }
        }

        private ObservableCollection<AiModelEvaluation> _aiModelEvaluations;

        [InverseProperty("AiRegistryModelVersion")]
        public virtual ObservableCollection<AiModelEvaluation> AiModelEvaluations
        {
            get
            {
                if (_aiModelEvaluations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiModelEvaluations - no database context is set. AiRegistryModelVersionId: " + this.AiRegistryModelVersionId + ".");
                        }
                        _aiModelEvaluations = new ObservableCollection<AiModelEvaluation>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiModelEvaluations.Where(x => x.ModelVersion == this.AiRegistryModelVersionId).ToList<AiModelEvaluation>();
                        _aiModelEvaluations = new ObservableCollection<AiModelEvaluation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiModelEvaluations.CollectionChanged += AiModelEvaluations_CollectionChanged;
                }
                return _aiModelEvaluations;
            }
            private set
            {
                if (_aiModelEvaluations != null)
                {
                    _aiModelEvaluations.CollectionChanged -= AiModelEvaluations_CollectionChanged;
                }
                _aiModelEvaluations = value;
                if (_aiModelEvaluations != null)
                {
                    _aiModelEvaluations.CollectionChanged += AiModelEvaluations_CollectionChanged;
                }
            }
        }

        private void AiModelEvaluations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiModelEvaluation>())
                {
                    item.ModelVersion = this.AiRegistryModelVersionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AiModelDeployments;
            _ = this.AiModelEvaluations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
