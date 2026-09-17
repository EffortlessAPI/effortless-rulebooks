
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
    [Table("ExternalStandardTerms")]
    public class ExternalStandardTermBase : SoAEntityBase
    {
        [Key]
        public string ExternalStandardTermId { get; set; }

        // Formula Name (rulebook: ={{TermIri}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.TermIri))); set { }
        }

        public string? TermIri { get; set; }
        public string? PreviousTermIri { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        public bool? DeprecatedBySource { get; set; }
        public DateTimeOffset? DeprecatedAt { get; set; }
        public bool? StillResolves { get; set; }
        public DateTimeOffset? AlignmentUpdatedAt { get; set; }
        public bool? ModelStillNeedsTerm { get; set; }
        // Formula RehomedTermSameAs (rulebook: =INDEX(VocabularyTerms!{{SameAsIri}}, MATCH({{RehomedAsTerm}}, VocabularyTerms!{{VocabularyTermId}}, 0)))
        [NotMapped]
        public string? RehomedTermSameAs
        {
            get => F.AsString(F.Memo(this, "RehomedTermSameAs", () => F.Lookup<VocabularyTerm>(this, "VocabularyTerms", "VocabularyTermId", __c => __c.VocabularyTerms, __r => F.Of(__r.VocabularyTermId), F.Of(this.RehomedAsTerm), __r => F.Of(__r.SameAsIri), () => F.Of(new VocabularyTerm().SameAsIri)))); set { }
        }

        // Formula RehomedTermNamespace (rulebook: =INDEX(VocabularyTerms!{{NamespaceIri}}, MATCH({{RehomedAsTerm}}, VocabularyTerms!{{VocabularyTermId}}, 0)))
        [NotMapped]
        public string? RehomedTermNamespace
        {
            get => F.AsString(F.Memo(this, "RehomedTermNamespace", () => F.Lookup<VocabularyTerm>(this, "VocabularyTerms", "VocabularyTermId", __c => __c.VocabularyTerms, __r => F.Of(__r.VocabularyTermId), F.Of(this.RehomedAsTerm), __r => F.Of(__r.NamespaceIri), () => F.Of(new VocabularyTerm().NamespaceIri)))); set { }
        }

        // Formula RehomedTermReleaseIssuedAt (rulebook: =INDEX(VocabularyTerms!{{IntroducedReleaseIssuedAt}}, MATCH({{RehomedAsTerm}}, VocabularyTerms!{{VocabularyTermId}}, 0)))
        [NotMapped]
        public DateTimeOffset? RehomedTermReleaseIssuedAt
        {
            get => F.AsDateTime(F.Memo(this, "RehomedTermReleaseIssuedAt", () => F.Lookup<VocabularyTerm>(this, "VocabularyTerms", "VocabularyTermId", __c => __c.VocabularyTerms, __r => F.Of(__r.VocabularyTermId), F.Of(this.RehomedAsTerm), __r => F.Of(__r.IntroducedReleaseIssuedAt), () => F.Of(new VocabularyTerm().IntroducedReleaseIssuedAt)))); set { }
        }

        // Formula ProfileNamespaceIri (rulebook: =INDEX(OntologyProfiles!{{NamespaceIri}}, MATCH({{OntologyProfile}}, OntologyProfiles!{{OntologyProfileId}}, 0)))
        [NotMapped]
        public string? ProfileNamespaceIri
        {
            get => F.AsString(F.Memo(this, "ProfileNamespaceIri", () => F.Lookup<OntologyProfile>(this, "OntologyProfiles", "OntologyProfileId", __c => __c.OntologyProfiles, __r => F.Of(__r.OntologyProfileId), F.Of(this.OntologyProfile), __r => F.Of(__r.NamespaceIri), () => F.Of(new OntologyProfile().NamespaceIri)))); set { }
        }

        // Formula DaysSinceDeprecated (rulebook: =IF({{DeprecatedAt}} = "", 0, DATETIME_DIFF({{AsOfInstant}}, {{DeprecatedAt}}, "days")))
        [NotMapped]
        public int? DaysSinceDeprecated
        {
            get => F.AsInt(F.Memo(this, "DaysSinceDeprecated", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.DeprecatedAt)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.DeprecatedAt), F.S("days")))))); set { }
        }

        // Formula IsRecentDeprecation (rulebook: =AND({{DeprecatedBySource}}, {{DeprecatedAt}} <> "", {{DaysSinceDeprecated}} <= 730))
        [NotMapped]
        public bool? IsRecentDeprecation
        {
            get => F.AsBool(F.Memo(this, "IsRecentDeprecation", () => F.And(F.IsTrueV(F.Of(this.DeprecatedBySource)), F.Bool3(F.IsNotBlank(F.Of(this.DeprecatedAt))), F.Bool3(F.Cmp(F.Of(this.DaysSinceDeprecated), "<=", F.I(730)))))); set { }
        }

        // Formula UsingMappingCount (rulebook: =COUNTIFS(SemanticMappings!{{TargetIri}}, {{TermIri}}))
        [NotMapped]
        public int? UsingMappingCount
        {
            get => F.AsInt(F.Memo(this, "UsingMappingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SemanticMapping>(base.SoAContext, "SemanticMappings", __c => __c.SemanticMappings), __r => F.CritField(F.Of(__r.TargetIri), F.Of(this.TermIri))))))); set { }
        }

        // Formula StaleIdentifierMappingCount (rulebook: =COUNTIFS(SemanticMappings!{{TargetIri}}, {{PreviousTermIri}}))
        [NotMapped]
        public int? StaleIdentifierMappingCount
        {
            get => F.AsInt(F.Memo(this, "StaleIdentifierMappingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SemanticMapping>(base.SoAContext, "SemanticMappings", __c => __c.SemanticMappings), __r => F.CritField(F.Of(__r.TargetIri), F.Of(this.PreviousTermIri))))))); set { }
        }

        // Formula IsAdoptedButDeprecated (rulebook: =AND({{DeprecatedBySource}}, OR({{UsingMappingCount}} > 0, {{RehomedAsTerm}} <> "")))
        [NotMapped]
        public bool? IsAdoptedButDeprecated
        {
            get => F.AsBool(F.Memo(this, "IsAdoptedButDeprecated", () => F.And(F.IsTrueV(F.Of(this.DeprecatedBySource)), F.Bool3(F.Or(F.Bool3(F.Cmp(F.Of(this.UsingMappingCount), ">", F.I(0))), F.Bool3(F.IsNotBlank(F.Of(this.RehomedAsTerm)))))))); set { }
        }

        // Formula IsAlignmentStaleAfterDeprecation (rulebook: =AND({{DeprecatedBySource}}, OR({{AlignmentUpdatedAt}} = "", {{AlignmentUpdatedAt}} < {{DeprecatedAt}})))
        [NotMapped]
        public bool? IsAlignmentStaleAfterDeprecation
        {
            get => F.AsBool(F.Memo(this, "IsAlignmentStaleAfterDeprecation", () => F.And(F.IsTrueV(F.Of(this.DeprecatedBySource)), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.AlignmentUpdatedAt))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.AlignmentUpdatedAt)), "<", F.Nullif(F.Of(this.DeprecatedAt))))))))); set { }
        }

        // Formula IsNeededDeprecatedTermNotRehomed (rulebook: =AND({{DeprecatedBySource}}, {{ModelStillNeedsTerm}}, {{RehomedAsTerm}} = ""))
        [NotMapped]
        public bool? IsNeededDeprecatedTermNotRehomed
        {
            get => F.AsBool(F.Memo(this, "IsNeededDeprecatedTermNotRehomed", () => F.And(F.IsTrueV(F.Of(this.DeprecatedBySource)), F.IsTrueV(F.Of(this.ModelStillNeedsTerm)), F.Bool3(F.IsBlank(F.Of(this.RehomedAsTerm)))))); set { }
        }

        // Formula IsRehomedWithoutIdentityLink (rulebook: =AND({{RehomedAsTerm}} <> "", OR({{RehomedTermSameAs}} = "", {{RehomedTermSameAs}} <> {{TermIri}})))
        [NotMapped]
        public bool? IsRehomedWithoutIdentityLink
        {
            get => F.AsBool(F.Memo(this, "IsRehomedWithoutIdentityLink", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.RehomedAsTerm))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.RehomedTermSameAs))), F.Bool3(F.Ne(F.Of(this.RehomedTermSameAs), F.Nullif(F.Of(this.TermIri))))))))); set { }
        }

        // Formula IsRehomedWithoutNewRelease (rulebook: =AND({{RehomedAsTerm}} <> "", OR({{RehomedTermReleaseIssuedAt}} = "", {{RehomedTermReleaseIssuedAt}} < {{DeprecatedAt}})))
        [NotMapped]
        public bool? IsRehomedWithoutNewRelease
        {
            get => F.AsBool(F.Memo(this, "IsRehomedWithoutNewRelease", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.RehomedAsTerm))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.RehomedTermReleaseIssuedAt))), F.Bool3(F.Cmp(F.Of(this.RehomedTermReleaseIssuedAt), "<", F.Nullif(F.Of(this.DeprecatedAt))))))))); set { }
        }

        // Formula HasUnpropagatedIdentifierChange (rulebook: =AND({{PreviousTermIri}} <> "", {{StaleIdentifierMappingCount}} > 0))
        [NotMapped]
        public bool? HasUnpropagatedIdentifierChange
        {
            get => F.AsBool(F.Memo(this, "HasUnpropagatedIdentifierChange", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PreviousTermIri))), F.Bool3(F.Cmp(F.Of(this.StaleIdentifierMappingCount), ">", F.I(0)))))); set { }
        }

        // Formula RehomingKeptExternalNamespace (rulebook: =AND({{RehomedAsTerm}} <> "", {{RehomedTermNamespace}} = {{ProfileNamespaceIri}}))
        [NotMapped]
        public bool? RehomingKeptExternalNamespace
        {
            get => F.AsBool(F.Memo(this, "RehomingKeptExternalNamespace", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.RehomedAsTerm))), F.Bool3(F.Eq(F.Of(this.RehomedTermNamespace), F.Of(this.ProfileNamespaceIri)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? OntologyProfile { get; set; }
        public string? EvaluationContext { get; set; }
        public string? RehomedAsTerm { get; set; }

        private OntologyProfile _ontologyProfileRef;

        [ForeignKey("OntologyProfile")]
        public virtual OntologyProfile OntologyProfileRef
        {
            get
            {
                if (_ontologyProfileRef == null && !string.IsNullOrEmpty(OntologyProfile))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OntologyProfileRef - no database context is set. OntologyProfile: " + OntologyProfile + ".");
                        }
                        return null;
                    }
                    _ontologyProfileRef = base.SoAContext.OntologyProfiles.Find(OntologyProfile);
                    if (_ontologyProfileRef != null)
                    {
                        base.SoAContext.Attach(_ontologyProfileRef);
                    }
                }
                return _ontologyProfileRef;
            }
            set
            {
                if (_ontologyProfileRef != value)
                {
                    _ontologyProfileRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_ontologyProfileRef != null)
                    {
                        OntologyProfile = _ontologyProfileRef.OntologyProfileId;
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

        private VocabularyTerm _vocabularyTerm;

        [ForeignKey("RehomedAsTerm")]
        public virtual VocabularyTerm VocabularyTerm
        {
            get
            {
                if (_vocabularyTerm == null && !string.IsNullOrEmpty(RehomedAsTerm))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerm - no database context is set. RehomedAsTerm: " + RehomedAsTerm + ".");
                        }
                        return null;
                    }
                    _vocabularyTerm = base.SoAContext.VocabularyTerms.Find(RehomedAsTerm);
                    if (_vocabularyTerm != null)
                    {
                        base.SoAContext.Attach(_vocabularyTerm);
                    }
                }
                return _vocabularyTerm;
            }
            set
            {
                if (_vocabularyTerm != value)
                {
                    _vocabularyTerm = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabularyTerm != null)
                    {
                        RehomedAsTerm = _vocabularyTerm.VocabularyTermId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OntologyProfileRef;
            _ = this.EvaluationContextRef;
            _ = this.VocabularyTerm;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
