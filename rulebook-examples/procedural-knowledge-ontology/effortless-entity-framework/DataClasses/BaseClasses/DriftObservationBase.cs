
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
    [Table("DriftObservations")]
    public class DriftObservationBase : SoAEntityBase
    {
        [Key]
        public string DriftObservationId { get; set; }

        // Formula Name (rulebook: ={{GovernedModel}} & ": " & LEFT({{Description}}, 50))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.GovernedModel)), F.S(": "), F.Text(F.Left(F.Of(this.Description), F.I(50)))))); set { }
        }

        public DateTimeOffset? ObservedAt { get; set; }
        public string? DetectedBy { get; set; }
        public string? DriftKind { get; set; }
        public string? DriftCause { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
        // Formula ReleasePassedValidation (rulebook: =INDEX(RulebookReleases!{{PassedValidationAtRelease}}, MATCH({{SinceRelease}}, RulebookReleases!{{RulebookReleaseId}}, 0)))
        [NotMapped]
        public bool? ReleasePassedValidation
        {
            get => F.AsBool(F.Memo(this, "ReleasePassedValidation", () => F.Lookup<RulebookRelease>(this, "RulebookReleases", "RulebookReleaseId", __c => __c.RulebookReleases, __r => F.Of(__r.RulebookReleaseId), F.Of(this.SinceRelease), __r => F.Of(__r.PassedValidationAtRelease), () => F.Of(new RulebookRelease().PassedValidationAtRelease)))); set { }
        }

        // Formula ReleaseIssuedAt (rulebook: =INDEX(RulebookReleases!{{IssuedAt}}, MATCH({{SinceRelease}}, RulebookReleases!{{RulebookReleaseId}}, 0)))
        [NotMapped]
        public DateTimeOffset? ReleaseIssuedAt
        {
            get => F.AsDateTime(F.Memo(this, "ReleaseIssuedAt", () => F.Lookup<RulebookRelease>(this, "RulebookReleases", "RulebookReleaseId", __c => __c.RulebookReleases, __r => F.Of(__r.RulebookReleaseId), F.Of(this.SinceRelease), __r => F.Of(__r.IssuedAt), () => F.Of(new RulebookRelease().IssuedAt)))); set { }
        }

        // Formula IsOpenPracticeMismatch (rulebook: =AND({{DriftKind}} = "PracticeMismatch", {{ResolvedAt}} = ""))
        [NotMapped]
        public bool? IsOpenPracticeMismatch
        {
            get => F.AsBool(F.Memo(this, "IsOpenPracticeMismatch", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.DriftKind)), F.S("PracticeMismatch"))), F.Bool3(F.IsBlank(F.Of(this.ResolvedAt)))))); set { }
        }

        // Formula WentUndetectedByPassingSuite (rulebook: =AND({{SinceRelease}} <> "", {{ReleasePassedValidation}}, {{DetectedBy}} <> "ValidationSuite"))
        [NotMapped]
        public bool? WentUndetectedByPassingSuite
        {
            get => F.AsBool(F.Memo(this, "WentUndetectedByPassingSuite", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.SinceRelease))), F.Bool3(F.Of(this.ReleasePassedValidation)), F.Bool3(F.Ne(F.Nullif(F.Of(this.DetectedBy)), F.S("ValidationSuite")))))); set { }
        }

        // Formula DriftFollowsCleanRelease (rulebook: =AND({{SinceRelease}} <> "", {{ReleasePassedValidation}}, {{ObservedAt}} > {{ReleaseIssuedAt}}))
        [NotMapped]
        public bool? DriftFollowsCleanRelease
        {
            get => F.AsBool(F.Memo(this, "DriftFollowsCleanRelease", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.SinceRelease))), F.Bool3(F.Of(this.ReleasePassedValidation)), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ObservedAt)), ">", F.Of(this.ReleaseIssuedAt)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? ProcedureVersion { get; set; }
        public string? ObservedByAgent { get; set; }
        public string? SinceRelease { get; set; }

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

        private Agent _agent;

        [ForeignKey("ObservedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ObservedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ObservedByAgent: " + ObservedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ObservedByAgent);
                    if (_agent != null)
                    {
                        base.SoAContext.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agent != null)
                    {
                        ObservedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private RulebookRelease _rulebookRelease;

        [ForeignKey("SinceRelease")]
        public virtual RulebookRelease RulebookRelease
        {
            get
            {
                if (_rulebookRelease == null && !string.IsNullOrEmpty(SinceRelease))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookRelease - no database context is set. SinceRelease: " + SinceRelease + ".");
                        }
                        return null;
                    }
                    _rulebookRelease = base.SoAContext.RulebookReleases.Find(SinceRelease);
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
                        SinceRelease = _rulebookRelease.RulebookReleaseId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.ProcedureVersionRef;
            _ = this.Agent;
            _ = this.RulebookRelease;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
