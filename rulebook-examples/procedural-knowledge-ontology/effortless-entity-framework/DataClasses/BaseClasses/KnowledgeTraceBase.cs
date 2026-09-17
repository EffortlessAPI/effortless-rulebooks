
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
    [Table("KnowledgeTraces")]
    public class KnowledgeTraceBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeTraceId { get; set; }

        // Formula Name (rulebook: ={{TargetKind}} & " <- " & {{SourceMaterial}} & " (" & {{TraceRole}} & ")")
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.TargetKind)), F.S(" <- "), F.Text(F.Of(this.SourceMaterial)), F.S(" ("), F.Text(F.Of(this.TraceRole)), F.S(")")))); set { }
        }

        public string? TargetKind { get; set; }
        public string? TraceRole { get; set; }
        public string? TracedAspect { get; set; }
        public string? SourceStatement { get; set; }
        public string? DerivationRoute { get; set; }
        public DateTimeOffset? ValidatedAt { get; set; }
        public int? SourceStatedDurationMinutes { get; set; }
        // Formula SourceMaterialKind (rulebook: =INDEX(CollectedSourceMaterials!{{MaterialKind}}, MATCH({{SourceMaterial}}, CollectedSourceMaterials!{{CollectedSourceMaterialId}}, 0)))
        [NotMapped]
        public string? SourceMaterialKind
        {
            get => F.AsString(F.Memo(this, "SourceMaterialKind", () => F.Lookup<CollectedSourceMaterial>(this, "CollectedSourceMaterials", "CollectedSourceMaterialId", __c => __c.CollectedSourceMaterials, __r => F.Of(__r.CollectedSourceMaterialId), F.Of(this.SourceMaterial), __r => F.Of(__r.MaterialKind), () => F.Of(new CollectedSourceMaterial().MaterialKind)))); set { }
        }

        // Formula SourceCollectedAt (rulebook: =INDEX(CollectedSourceMaterials!{{CollectedAt}}, MATCH({{SourceMaterial}}, CollectedSourceMaterials!{{CollectedSourceMaterialId}}, 0)))
        [NotMapped]
        public DateTimeOffset? SourceCollectedAt
        {
            get => F.AsDateTime(F.Memo(this, "SourceCollectedAt", () => F.Lookup<CollectedSourceMaterial>(this, "CollectedSourceMaterials", "CollectedSourceMaterialId", __c => __c.CollectedSourceMaterials, __r => F.Of(__r.CollectedSourceMaterialId), F.Of(this.SourceMaterial), __r => F.Of(__r.CollectedAt), () => F.Of(new CollectedSourceMaterial().CollectedAt)))); set { }
        }

        // Formula SourceRevisedAt (rulebook: =INDEX(CollectedSourceMaterials!{{SourceDocumentRevisedAt}}, MATCH({{SourceMaterial}}, CollectedSourceMaterials!{{CollectedSourceMaterialId}}, 0)))
        [NotMapped]
        public DateTimeOffset? SourceRevisedAt
        {
            get => F.AsDateTime(F.Memo(this, "SourceRevisedAt", () => F.Lookup<CollectedSourceMaterial>(this, "CollectedSourceMaterials", "CollectedSourceMaterialId", __c => __c.CollectedSourceMaterials, __r => F.Of(__r.CollectedSourceMaterialId), F.Of(this.SourceMaterial), __r => F.Of(__r.SourceDocumentRevisedAt), () => F.Of(new CollectedSourceMaterial().SourceDocumentRevisedAt)))); set { }
        }

        // Formula SourceIsDocument (rulebook: =INDEX(CollectedSourceMaterials!{{IsDocumentSource}}, MATCH({{SourceMaterial}}, CollectedSourceMaterials!{{CollectedSourceMaterialId}}, 0)))
        [NotMapped]
        public bool? SourceIsDocument
        {
            get => F.AsBool(F.Memo(this, "SourceIsDocument", () => F.Lookup<CollectedSourceMaterial>(this, "CollectedSourceMaterials", "CollectedSourceMaterialId", __c => __c.CollectedSourceMaterials, __r => F.Of(__r.CollectedSourceMaterialId), F.Of(this.SourceMaterial), __r => F.Of(__r.IsDocumentSource), () => F.Of(new CollectedSourceMaterial().IsDocumentSource)))); set { }
        }

        // Formula SourceIsPeopleCapture (rulebook: =INDEX(CollectedSourceMaterials!{{IsPeopleCapture}}, MATCH({{SourceMaterial}}, CollectedSourceMaterials!{{CollectedSourceMaterialId}}, 0)))
        [NotMapped]
        public bool? SourceIsPeopleCapture
        {
            get => F.AsBool(F.Memo(this, "SourceIsPeopleCapture", () => F.Lookup<CollectedSourceMaterial>(this, "CollectedSourceMaterials", "CollectedSourceMaterialId", __c => __c.CollectedSourceMaterials, __r => F.Of(__r.CollectedSourceMaterialId), F.Of(this.SourceMaterial), __r => F.Of(__r.IsPeopleCapture), () => F.Of(new CollectedSourceMaterial().IsPeopleCapture)))); set { }
        }

        // Formula SourceIsPracticeEvidence (rulebook: =INDEX(CollectedSourceMaterials!{{IsPracticeEvidence}}, MATCH({{SourceMaterial}}, CollectedSourceMaterials!{{CollectedSourceMaterialId}}, 0)))
        [NotMapped]
        public bool? SourceIsPracticeEvidence
        {
            get => F.AsBool(F.Memo(this, "SourceIsPracticeEvidence", () => F.Lookup<CollectedSourceMaterial>(this, "CollectedSourceMaterials", "CollectedSourceMaterialId", __c => __c.CollectedSourceMaterials, __r => F.Of(__r.CollectedSourceMaterialId), F.Of(this.SourceMaterial), __r => F.Of(__r.IsPracticeEvidence), () => F.Of(new CollectedSourceMaterial().IsPracticeEvidence)))); set { }
        }

        // Formula IsSourceChangedSinceTaken (rulebook: =AND({{SourceRevisedAt}} <> "", {{SourceRevisedAt}} > {{SourceCollectedAt}}))
        [NotMapped]
        public bool? IsSourceChangedSinceTaken
        {
            get => F.AsBool(F.Memo(this, "IsSourceChangedSinceTaken", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.SourceRevisedAt))), F.Bool3(F.Cmp(F.Of(this.SourceRevisedAt), ">", F.Of(this.SourceCollectedAt)))))); set { }
        }

        // Formula ModeledDurationMinutes (rulebook: =INDEX(Steps!{{ExpectedDurationMinutes}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public int? ModeledDurationMinutes
        {
            get => F.AsInt(F.Memo(this, "ModeledDurationMinutes", () => F.Integer(F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.ExpectedDurationMinutes), () => F.Of(new Step().ExpectedDurationMinutes))))); set { }
        }

        // Formula IsUnfaithfulToSource (rulebook: =AND({{Step}} <> "", {{SourceStatedDurationMinutes}} > 0, {{ModeledDurationMinutes}} <> {{SourceStatedDurationMinutes}}))
        [NotMapped]
        public bool? IsUnfaithfulToSource
        {
            get => F.AsBool(F.Memo(this, "IsUnfaithfulToSource", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.Step))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.SourceStatedDurationMinutes)), ">", F.I(0))), F.Bool3(F.Ne(F.Of(this.ModeledDurationMinutes), F.Nullif(F.Of(this.SourceStatedDurationMinutes))))))); set { }
        }

        // Formula DerivedByAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{DerivedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? DerivedByAgentKind
        {
            get => F.AsString(F.Memo(this, "DerivedByAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.DerivedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula IsMachineDerived (rulebook: =AND({{DerivedByAgent}} <> "", {{DerivedByAgentKind}} <> "Human"))
        [NotMapped]
        public bool? IsMachineDerived
        {
            get => F.AsBool(F.Memo(this, "IsMachineDerived", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.DerivedByAgent))), F.Bool3(F.Ne(F.Of(this.DerivedByAgentKind), F.S("Human")))))); set { }
        }

        // Formula IsSelfValidated (rulebook: =AND({{ValidatedByAgent}} <> "", {{ValidatedByAgent}} = {{DerivedByAgent}}))
        [NotMapped]
        public bool? IsSelfValidated
        {
            get => F.AsBool(F.Memo(this, "IsSelfValidated", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ValidatedByAgent))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ValidatedByAgent)), F.Nullif(F.Of(this.DerivedByAgent))))))); set { }
        }

        // Formula HasIncompleteProvenance (rulebook: =OR({{SourceMaterial}} = "", {{DerivationRoute}} = "", {{ValidatedByAgent}} = ""))
        [NotMapped]
        public bool? HasIncompleteProvenance
        {
            get => F.AsBool(F.Memo(this, "HasIncompleteProvenance", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.SourceMaterial))), F.Bool3(F.IsBlank(F.Of(this.DerivationRoute))), F.Bool3(F.IsBlank(F.Of(this.ValidatedByAgent)))))); set { }
        }

        // Formula ProvenanceStatement (rulebook: ="From " & {{SourceMaterial}} & "; route: " & {{DerivationRoute}} & "; derived by " & {{DerivedByAgent}} & "; confirmed by " & {{ValidatedByAgent}})
        [NotMapped]
        public string? ProvenanceStatement
        {
            get => F.AsString(F.Memo(this, "ProvenanceStatement", () => F.Concat(F.S("From "), F.Text(F.Of(this.SourceMaterial)), F.S("; route: "), F.Text(F.Of(this.DerivationRoute)), F.S("; derived by "), F.Text(F.Of(this.DerivedByAgent)), F.S("; confirmed by "), F.Text(F.Of(this.ValidatedByAgent))))); set { }
        }

        // Formula IsAspectUnsupportedBySourceKind (rulebook: =OR(AND({{TracedAspect}} = "Workaround", {{SourceMaterialKind}} <> "FieldNotes"), AND({{TracedAspect}} = "ActualExecution", {{SourceMaterialKind}} <> "MinedEventTrace")))
        [NotMapped]
        public bool? IsAspectUnsupportedBySourceKind
        {
            get => F.AsBool(F.Memo(this, "IsAspectUnsupportedBySourceKind", () => F.Or(F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.TracedAspect)), F.S("Workaround"))), F.Bool3(F.Ne(F.Of(this.SourceMaterialKind), F.S("FieldNotes"))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.TracedAspect)), F.S("ActualExecution"))), F.Bool3(F.Ne(F.Of(this.SourceMaterialKind), F.S("MinedEventTrace")))))))); set { }
        }

        // Formula IsDocumentOrigin (rulebook: =AND({{TraceRole}} = "Origin", {{SourceIsDocument}}))
        [NotMapped]
        public bool? IsDocumentOrigin
        {
            get => F.AsBool(F.Memo(this, "IsDocumentOrigin", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.TraceRole)), F.S("Origin"))), F.Bool3(F.Of(this.SourceIsDocument))))); set { }
        }

        // Formula StepElicitedValidationCount (rulebook: =INDEX(Steps!{{ElicitedValidationCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public int? StepElicitedValidationCount
        {
            get => F.AsInt(F.Memo(this, "StepElicitedValidationCount", () => F.Integer(F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.ElicitedValidationCount), () => F.Of(new Step().ElicitedValidationCount))))); set { }
        }

        // Formula StepElicitedExtensionCount (rulebook: =INDEX(Steps!{{ElicitedExtensionCount}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public int? StepElicitedExtensionCount
        {
            get => F.AsInt(F.Memo(this, "StepElicitedExtensionCount", () => F.Integer(F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.ElicitedExtensionCount), () => F.Of(new Step().ElicitedExtensionCount))))); set { }
        }

        // Formula IsDocumentStartNeverValidated (rulebook: =AND({{IsDocumentOrigin}}, {{Step}} <> "", {{StepElicitedValidationCount}} = 0))
        [NotMapped]
        public bool? IsDocumentStartNeverValidated
        {
            get => F.AsBool(F.Memo(this, "IsDocumentStartNeverValidated", () => F.And(F.Bool3(F.Of(this.IsDocumentOrigin)), F.Bool3(F.IsNotBlank(F.Of(this.Step))), F.Bool3(F.Eq(F.Of(this.StepElicitedValidationCount), F.I(0)))))); set { }
        }

        // Formula IsDocumentStartNeverExtended (rulebook: =AND({{IsDocumentOrigin}}, {{Step}} <> "", {{StepElicitedExtensionCount}} = 0))
        [NotMapped]
        public bool? IsDocumentStartNeverExtended
        {
            get => F.AsBool(F.Memo(this, "IsDocumentStartNeverExtended", () => F.And(F.Bool3(F.Of(this.IsDocumentOrigin)), F.Bool3(F.IsNotBlank(F.Of(this.Step))), F.Bool3(F.Eq(F.Of(this.StepElicitedExtensionCount), F.I(0)))))); set { }
        }

        // Formula ContradictedDocumentRevisedAt (rulebook: =INDEX(Resources!{{ModifiedAt}}, MATCH({{ContradictedDocument}}, Resources!{{ResourceId}}, 0)))
        [NotMapped]
        public DateTimeOffset? ContradictedDocumentRevisedAt
        {
            get => F.AsDateTime(F.Memo(this, "ContradictedDocumentRevisedAt", () => F.Lookup<Resource>(this, "Resources", "ResourceId", __c => __c.Resources, __r => F.Of(__r.ResourceId), F.Of(this.ContradictedDocument), __r => F.Of(__r.ModifiedAt), () => F.Of(new Resource().ModifiedAt)))); set { }
        }

        // Formula IsDocumentTrailingPractice (rulebook: =AND({{TraceRole}} = "Contradicts", {{ContradictedDocument}} <> "", {{SourceIsPracticeEvidence}}, {{SourceCollectedAt}} > {{ContradictedDocumentRevisedAt}}))
        [NotMapped]
        public bool? IsDocumentTrailingPractice
        {
            get => F.AsBool(F.Memo(this, "IsDocumentTrailingPractice", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.TraceRole)), F.S("Contradicts"))), F.Bool3(F.IsNotBlank(F.Of(this.ContradictedDocument))), F.Bool3(F.Of(this.SourceIsPracticeEvidence)), F.Bool3(F.Cmp(F.Of(this.SourceCollectedAt), ">", F.Of(this.ContradictedDocumentRevisedAt)))))); set { }
        }

        // Formula PrescribedVersusEnacted (rulebook: =IF({{ContradictedDocument}} = "", "", "Prescribed in " & {{ContradictedDocument}} & "; enacted per " & {{SourceMaterial}} & ": " & {{SourceStatement}}))
        [NotMapped]
        public string? PrescribedVersusEnacted
        {
            get => F.AsString(F.Memo(this, "PrescribedVersusEnacted", () => (F.Truthy(F.Bool3(F.IsBlank(F.Of(this.ContradictedDocument)))) ? F.S("") : F.Concat(F.S("Prescribed in "), F.Text(F.Of(this.ContradictedDocument)), F.S("; enacted per "), F.Text(F.Of(this.SourceMaterial)), F.S(": "), F.Text(F.Of(this.SourceStatement)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? Requirement { get; set; }
        public string? SourceMaterial { get; set; }
        public string? DerivedByAgent { get; set; }
        public string? ValidatedByAgent { get; set; }
        public string? ContradictedDocument { get; set; }

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

        private Requirement _requirementRef;

        [ForeignKey("Requirement")]
        public virtual Requirement RequirementRef
        {
            get
            {
                if (_requirementRef == null && !string.IsNullOrEmpty(Requirement))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequirementRef - no database context is set. Requirement: " + Requirement + ".");
                        }
                        return null;
                    }
                    _requirementRef = base.SoAContext.Requirements.Find(Requirement);
                    if (_requirementRef != null)
                    {
                        base.SoAContext.Attach(_requirementRef);
                    }
                }
                return _requirementRef;
            }
            set
            {
                if (_requirementRef != value)
                {
                    _requirementRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_requirementRef != null)
                    {
                        Requirement = _requirementRef.RequirementId;
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

        private Agent _agent;

        [ForeignKey("DerivedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(DerivedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. DerivedByAgent: " + DerivedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(DerivedByAgent);
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
                        DerivedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("ValidatedByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(ValidatedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. ValidatedByAgent: " + ValidatedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(ValidatedByAgent);
                    if (_agentRef != null)
                    {
                        base.SoAContext.Attach(_agentRef);
                    }
                }
                return _agentRef;
            }
            set
            {
                if (_agentRef != value)
                {
                    _agentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRef != null)
                    {
                        ValidatedByAgent = _agentRef.AgentId;
                    }
                }
            }
        }

        private Resource _resource;

        [ForeignKey("ContradictedDocument")]
        public virtual Resource Resource
        {
            get
            {
                if (_resource == null && !string.IsNullOrEmpty(ContradictedDocument))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. ContradictedDocument: " + ContradictedDocument + ".");
                        }
                        return null;
                    }
                    _resource = base.SoAContext.Resources.Find(ContradictedDocument);
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
                        ContradictedDocument = _resource.ResourceId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.StepRef;
            _ = this.RequirementRef;
            _ = this.CollectedSourceMaterial;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.Resource;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
