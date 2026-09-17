
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
    [Table("ModelDocuments")]
    public class ModelDocumentBase : SoAEntityBase
    {
        [Key]
        public string ModelDocumentId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Title))); set { }
        }

        public string? Title { get; set; }
        public string? DocumentKind { get; set; }
        public DateTimeOffset? LastUpdatedAt { get; set; }
        // Formula ModelCurrentRelease (rulebook: =INDEX(GovernedModels!{{CurrentRelease}}, MATCH({{GovernedModel}}, GovernedModels!{{GovernedModelId}}, 0)))
        [NotMapped]
        public string? ModelCurrentRelease
        {
            get => F.AsString(F.Memo(this, "ModelCurrentRelease", () => F.Lookup<GovernedModel>(this, "GovernedModels", "GovernedModelId", __c => __c.GovernedModels, __r => F.Of(__r.GovernedModelId), F.Of(this.GovernedModel), __r => F.Of(__r.CurrentRelease), () => F.Of(new GovernedModel().CurrentRelease)))); set { }
        }

        // Formula IsBehindCurrentRelease (rulebook: =AND({{DocumentedRelease}} <> "", {{ModelCurrentRelease}} <> "", {{DocumentedRelease}} <> {{ModelCurrentRelease}}))
        [NotMapped]
        public bool? IsBehindCurrentRelease
        {
            get => F.AsBool(F.Memo(this, "IsBehindCurrentRelease", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.DocumentedRelease))), F.Bool3(F.IsNotBlank(F.Of(this.ModelCurrentRelease))), F.Bool3(F.Ne(F.Nullif(F.Of(this.DocumentedRelease)), F.Of(this.ModelCurrentRelease)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? DocumentedRelease { get; set; }

        private GovernedModel _governedModelRef;

        [ForeignKey("GovernedModel")]
        public virtual GovernedModel GovernedModelRef
        {
            get
            {
                if (_governedModelRef == null && !string.IsNullOrEmpty(GovernedModel))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModelRef - no database context is set. GovernedModel: " + GovernedModel + ".");
                        }
                        return null;
                    }
                    _governedModelRef = base.SoAContext.GovernedModels.Find(GovernedModel);
                    if (_governedModelRef != null)
                    {
                        base.SoAContext.Attach(_governedModelRef);
                    }
                }
                return _governedModelRef;
            }
            set
            {
                if (_governedModelRef != value)
                {
                    _governedModelRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_governedModelRef != null)
                    {
                        GovernedModel = _governedModelRef.GovernedModelId;
                    }
                }
            }
        }

        private RulebookRelease _rulebookRelease;

        [ForeignKey("DocumentedRelease")]
        public virtual RulebookRelease RulebookRelease
        {
            get
            {
                if (_rulebookRelease == null && !string.IsNullOrEmpty(DocumentedRelease))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookRelease - no database context is set. DocumentedRelease: " + DocumentedRelease + ".");
                        }
                        return null;
                    }
                    _rulebookRelease = base.SoAContext.RulebookReleases.Find(DocumentedRelease);
                    if (_rulebookRelease != null)
                    {
                        base.SoAContext.Attach(_rulebookRelease);
                    }
                }
                return _rulebookRelease;
            }
            set
            {
                if (_rulebookRelease != value)
                {
                    _rulebookRelease = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookRelease != null)
                    {
                        DocumentedRelease = _rulebookRelease.RulebookReleaseId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.RulebookRelease;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
