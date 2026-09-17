
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
    [Table("AiAdoptionInitiatives")]
    public class AiAdoptionInitiatifBase : SoAEntityBase
    {
        [Key]
        public string AiAdoptionInitiativeId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Approach { get; set; }
        public string? AutomationLevel { get; set; }
        public bool? IsAgentic { get; set; }
        public bool? TaskIsMultiStep { get; set; }
        public bool? DependsOnOrganizationalKnowledge { get; set; }
        public string? Status { get; set; }
        public string? Outcome { get; set; }
        public DateTimeOffset? RedesignStartedAt { get; set; }
        public DateTimeOffset? ConnectedToAiAt { get; set; }
        public decimal? ModelLayerBudget { get; set; }
        public decimal? KnowledgeLayerBudget { get; set; }
        public decimal? MeasuredBottomLineImpact { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula TargetHasNoExplicitSteps (rulebook: =INDEX(Procedures!{{HasNoExplicitSteps}}, MATCH({{TargetProcedure}}, Procedures!{{ProcedureId}}, 0)))
        [NotMapped]
        public bool? TargetHasNoExplicitSteps
        {
            get => F.AsBool(F.Memo(this, "TargetHasNoExplicitSteps", () => F.Lookup<Procedure>(this, "Procedures", "ProcedureId", __c => __c.Procedures, __r => F.Of(__r.ProcedureId), F.Of(this.TargetProcedure), __r => F.Of(__r.HasNoExplicitSteps), () => F.Of(new Procedure().HasNoExplicitSteps)))); set { }
        }

        // Formula TargetVersionIssuedAt (rulebook: =INDEX(ProcedureVersions!{{IssuedAt}}, MATCH({{TargetVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public DateTimeOffset? TargetVersionIssuedAt
        {
            get => F.AsDateTime(F.Memo(this, "TargetVersionIssuedAt", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.TargetVersion), __r => F.Of(__r.IssuedAt), () => F.Of(new ProcedureVersion().IssuedAt)))); set { }
        }

        // Formula TargetVersionUnderSpecified (rulebook: =INDEX(ProcedureVersions!{{IsUnderSpecifiedForExecution}}, MATCH({{TargetVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public bool? TargetVersionUnderSpecified
        {
            get => F.AsBool(F.Memo(this, "TargetVersionUnderSpecified", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.TargetVersion), __r => F.Of(__r.IsUnderSpecifiedForExecution), () => F.Of(new ProcedureVersion().IsUnderSpecifiedForExecution)))); set { }
        }

        // Formula TargetVersionNotationOnly (rulebook: =INDEX(ProcedureVersions!{{RestsOnNotationOnly}}, MATCH({{TargetVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public bool? TargetVersionNotationOnly
        {
            get => F.AsBool(F.Memo(this, "TargetVersionNotationOnly", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.TargetVersion), __r => F.Of(__r.RestsOnNotationOnly), () => F.Of(new ProcedureVersion().RestsOnNotationOnly)))); set { }
        }

        // Formula TargetElicitationEvidenceCount (rulebook: =INDEX(ProcedureVersions!{{ElicitationEvidenceCount}}, MATCH({{TargetVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public int? TargetElicitationEvidenceCount
        {
            get => F.AsInt(F.Memo(this, "TargetElicitationEvidenceCount", () => F.Integer(F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.TargetVersion), __r => F.Of(__r.ElicitationEvidenceCount), () => F.Of(new ProcedureVersion().ElicitationEvidenceCount))))); set { }
        }

        // Formula ReviewedOutputCount (rulebook: =COUNTIFS(AssistantAnswers!{{ReviewedForInitiative}}, {{AiAdoptionInitiativeId}}))
        [NotMapped]
        public int? ReviewedOutputCount
        {
            get => F.AsInt(F.Memo(this, "ReviewedOutputCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AssistantAnswer>(base.SoAContext, "AssistantAnswers", __c => __c.AssistantAnswers), __r => F.CritField(F.Of(__r.ReviewedForInitiative), F.Of(this.AiAdoptionInitiativeId))))))); set { }
        }

        // Formula PrecedingReviewedOutputCount (rulebook: =INDEX(AiAdoptionInitiatives!{{ReviewedOutputCount}}, MATCH({{PrecedingInitiative}}, AiAdoptionInitiatives!{{AiAdoptionInitiativeId}}, 0)))
        [NotMapped]
        public int? PrecedingReviewedOutputCount
        {
            get => F.AsInt(F.Memo(this, "PrecedingReviewedOutputCount", () => F.Integer(F.Lookup<AiAdoptionInitiatif>(this, "AiAdoptionInitiatives", "AiAdoptionInitiativeId", __c => __c.AiAdoptionInitiatives, __r => F.Of(__r.AiAdoptionInitiativeId), F.Of(this.PrecedingInitiative), __r => F.Of(__r.ReviewedOutputCount), () => F.Of(new AiAdoptionInitiatif().ReviewedOutputCount))))); set { }
        }

        // Formula LastOutcomeMeasuredAt (rulebook: =MAXIFS(KnowledgeOutcomeMeasurements!{{MeasuredAt}}, KnowledgeOutcomeMeasurements!{{AiInitiative}}, {{AiAdoptionInitiativeId}}))
        [NotMapped]
        public DateTimeOffset? LastOutcomeMeasuredAt
        {
            get => F.AsDateTime(F.Memo(this, "LastOutcomeMeasuredAt", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<KnowledgeOutcomeMeasurement>(base.SoAContext, "KnowledgeOutcomeMeasurements", __c => __c.KnowledgeOutcomeMeasurements), __r => F.CritField(F.Of(__r.AiInitiative), F.Of(this.AiAdoptionInitiativeId)), __r => F.Of(__r.MeasuredAt))))); set { }
        }

        // Formula DaysSinceOutcomeMeasured (rulebook: =IF({{LastOutcomeMeasuredAt}} = "", 0, DATETIME_DIFF({{AsOfInstant}}, {{LastOutcomeMeasuredAt}}, "days")))
        [NotMapped]
        public int? DaysSinceOutcomeMeasured
        {
            get => F.AsInt(F.Memo(this, "DaysSinceOutcomeMeasured", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.LastOutcomeMeasuredAt)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.LastOutcomeMeasuredAt), F.S("days")))))); set { }
        }

        // Formula AiInsightCount (rulebook: =COUNTIFS(AiInsightProposals!{{SourceInitiative}}, {{AiAdoptionInitiativeId}}))
        [NotMapped]
        public int? AiInsightCount
        {
            get => F.AsInt(F.Memo(this, "AiInsightCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AiInsightProposal>(base.SoAContext, "AiInsightProposals", __c => __c.AiInsightProposals), __r => F.CritField(F.Of(__r.SourceInitiative), F.Of(this.AiAdoptionInitiativeId))))))); set { }
        }

        // Formula HandsTacitProcedureToAgent (rulebook: =AND({{IsAgentic}}, {{TargetProcedure}} <> "", {{TargetHasNoExplicitSteps}}))
        [NotMapped]
        public bool? HandsTacitProcedureToAgent
        {
            get => F.AsBool(F.Memo(this, "HandsTacitProcedureToAgent", () => F.And(F.IsTrueV(F.Of(this.IsAgentic)), F.Bool3(F.IsNotBlank(F.Of(this.TargetProcedure))), F.Bool3(F.Of(this.TargetHasNoExplicitSteps))))); set { }
        }

        // Formula AgentOnUnderSpecifiedProcedure (rulebook: =AND({{IsAgentic}}, {{TargetVersion}} <> "", {{TargetVersionUnderSpecified}}))
        [NotMapped]
        public bool? AgentOnUnderSpecifiedProcedure
        {
            get => F.AsBool(F.Memo(this, "AgentOnUnderSpecifiedProcedure", () => F.And(F.IsTrueV(F.Of(this.IsAgentic)), F.Bool3(F.IsNotBlank(F.Of(this.TargetVersion))), F.Bool3(F.Of(this.TargetVersionUnderSpecified))))); set { }
        }

        // Formula AgentOnNotationOnlyProcedure (rulebook: =AND({{IsAgentic}}, {{TargetVersion}} <> "", {{TargetVersionNotationOnly}}))
        [NotMapped]
        public bool? AgentOnNotationOnlyProcedure
        {
            get => F.AsBool(F.Memo(this, "AgentOnNotationOnlyProcedure", () => F.And(F.IsTrueV(F.Of(this.IsAgentic)), F.Bool3(F.IsNotBlank(F.Of(this.TargetVersion))), F.Bool3(F.Of(this.TargetVersionNotationOnly))))); set { }
        }

        // Formula CallsForProcessKnowledgeFramework (rulebook: =AND({{TaskIsMultiStep}}, {{DependsOnOrganizationalKnowledge}}))
        [NotMapped]
        public bool? CallsForProcessKnowledgeFramework
        {
            get => F.AsBool(F.Memo(this, "CallsForProcessKnowledgeFramework", () => F.And(F.IsTrueV(F.Of(this.TaskIsMultiStep)), F.IsTrueV(F.Of(this.DependsOnOrganizationalKnowledge))))); set { }
        }

        // Formula RedesignedBeforeDocumented (rulebook: =AND({{RedesignStartedAt}} <> "", OR({{TargetVersion}} = "", {{RedesignStartedAt}} < {{TargetVersionIssuedAt}})))
        [NotMapped]
        public bool? RedesignedBeforeDocumented
        {
            get => F.AsBool(F.Memo(this, "RedesignedBeforeDocumented", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.RedesignStartedAt))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.TargetVersion))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.RedesignStartedAt)), "<", F.Of(this.TargetVersionIssuedAt)))))))); set { }
        }

        // Formula BreaksElicitEncodeConnectOrder (rulebook: =AND({{ConnectedToAiAt}} <> "", OR({{TargetVersion}} = "", {{TargetElicitationEvidenceCount}} = 0, {{ConnectedToAiAt}} < {{TargetVersionIssuedAt}})))
        [NotMapped]
        public bool? BreaksElicitEncodeConnectOrder
        {
            get => F.AsBool(F.Memo(this, "BreaksElicitEncodeConnectOrder", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ConnectedToAiAt))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.TargetVersion))), F.Bool3(F.Eq(F.Of(this.TargetElicitationEvidenceCount), F.I(0))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ConnectedToAiAt)), "<", F.Of(this.TargetVersionIssuedAt)))))))); set { }
        }

        // Formula WentFullWithoutReviewedPartialStage (rulebook: =AND({{AutomationLevel}} = "Full", OR({{PrecedingInitiative}} = "", {{PrecedingReviewedOutputCount}} = 0)))
        [NotMapped]
        public bool? WentFullWithoutReviewedPartialStage
        {
            get => F.AsBool(F.Memo(this, "WentFullWithoutReviewedPartialStage", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AutomationLevel)), F.S("Full"))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.PrecedingInitiative))), F.Bool3(F.Eq(F.Of(this.PrecedingReviewedOutputCount), F.I(0)))))))); set { }
        }

        // Formula UnderinvestsKnowledgeLayer (rulebook: =AND({{ModelLayerBudget}} > 0, {{KnowledgeLayerBudget}} < {{ModelLayerBudget}}))
        [NotMapped]
        public bool? UnderinvestsKnowledgeLayer
        {
            get => F.AsBool(F.Memo(this, "UnderinvestsKnowledgeLayer", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ModelLayerBudget)), ">", F.I(0))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.KnowledgeLayerBudget)), "<", F.Nullif(F.Of(this.ModelLayerBudget))))))); set { }
        }

        // Formula NotAnchoredInProcessKnowledge (rulebook: =AND({{Status}} <> "Proposed", {{TargetProcedure}} = ""))
        [NotMapped]
        public bool? NotAnchoredInProcessKnowledge
        {
            get => F.AsBool(F.Memo(this, "NotAnchoredInProcessKnowledge", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.Status)), F.S("Proposed"))), F.Bool3(F.IsBlank(F.Of(this.TargetProcedure)))))); set { }
        }

        // Formula AgenticWithoutKnowledgeCapture (rulebook: =AND({{IsAgentic}}, {{Status}} <> "Proposed", {{AiInsightCount}} = 0))
        [NotMapped]
        public bool? AgenticWithoutKnowledgeCapture
        {
            get => F.AsBool(F.Memo(this, "AgenticWithoutKnowledgeCapture", () => F.And(F.IsTrueV(F.Of(this.IsAgentic)), F.Bool3(F.Ne(F.Nullif(F.Of(this.Status)), F.S("Proposed"))), F.Bool3(F.Eq(F.Of(this.AiInsightCount), F.I(0)))))); set { }
        }

        // Formula IsAiOutcomeUnmeasured (rulebook: =AND(OR({{Status}} = "Pilot", {{Status}} = "InProduction"), OR({{LastOutcomeMeasuredAt}} = "", {{DaysSinceOutcomeMeasured}} > 30)))
        [NotMapped]
        public bool? IsAiOutcomeUnmeasured
        {
            get => F.AsBool(F.Memo(this, "IsAiOutcomeUnmeasured", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Pilot"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("InProduction"))))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.LastOutcomeMeasuredAt))), F.Bool3(F.Cmp(F.Of(this.DaysSinceOutcomeMeasured), ">", F.I(30)))))))); set { }
        }

        // Formula AdoptedWithoutBottomLineResult (rulebook: =AND({{Status}} = "InProduction", {{MeasuredBottomLineImpact}} = 0))
        [NotMapped]
        public bool? AdoptedWithoutBottomLineResult
        {
            get => F.AsBool(F.Memo(this, "AdoptedWithoutBottomLineResult", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("InProduction"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.MeasuredBottomLineImpact)), F.I(0)))))); set { }
        }

        // Formula FailedWithoutFormalizedKnowledge (rulebook: =AND({{Outcome}} = "Failed", {{Approach}} = "PromptOnly", OR({{TargetProcedure}} = "", AND({{TargetProcedure}} <> "", {{TargetHasNoExplicitSteps}}))))
        [NotMapped]
        public bool? FailedWithoutFormalizedKnowledge
        {
            get => F.AsBool(F.Memo(this, "FailedWithoutFormalizedKnowledge", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Outcome)), F.S("Failed"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Approach)), F.S("PromptOnly"))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.TargetProcedure))), F.Bool3(F.And(F.Bool3(F.IsNotBlank(F.Of(this.TargetProcedure))), F.Bool3(F.Of(this.TargetHasNoExplicitSteps))))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }
        public string? Agent { get; set; }
        public string? TargetProcedure { get; set; }
        public string? TargetVersion { get; set; }
        public string? PrecedingInitiative { get; set; }
        public string? EvaluationContext { get; set; }

        private Organization _organizationRef;

        [ForeignKey("Organization")]
        public virtual Organization OrganizationRef
        {
            get
            {
                if (_organizationRef == null && !string.IsNullOrEmpty(Organization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRef - no database context is set. Organization: " + Organization + ".");
                        }
                        return null;
                    }
                    _organizationRef = base.SoAContext.Organizations.Find(Organization);
                    if (_organizationRef != null)
                    {
                        base.SoAContext.Attach(_organizationRef);
                    }
                }
                return _organizationRef;
            }
            set
            {
                if (_organizationRef != value)
                {
                    _organizationRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organizationRef != null)
                    {
                        Organization = _organizationRef.OrganizationId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("Agent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(Agent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. Agent: " + Agent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(Agent);
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
                        Agent = _agentRef.AgentId;
                    }
                }
            }
        }

        private Procedure _procedure;

        [ForeignKey("TargetProcedure")]
        public virtual Procedure Procedure
        {
            get
            {
                if (_procedure == null && !string.IsNullOrEmpty(TargetProcedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedure - no database context is set. TargetProcedure: " + TargetProcedure + ".");
                        }
                        return null;
                    }
                    _procedure = base.SoAContext.Procedures.Find(TargetProcedure);
                    if (_procedure != null)
                    {
                        base.SoAContext.Attach(_procedure);
                    }
                }
                return _procedure;
            }
            set
            {
                if (_procedure != value)
                {
                    _procedure = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedure != null)
                    {
                        TargetProcedure = _procedure.ProcedureId;
                    }
                }
            }
        }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("TargetVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(TargetVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. TargetVersion: " + TargetVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = base.SoAContext.ProcedureVersions.Find(TargetVersion);
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
                        TargetVersion = _procedureVersion.ProcedureVersionId;
                    }
                }
            }
        }

        private AiAdoptionInitiatif _aiAdoptionInitiatif;

        [ForeignKey("PrecedingInitiative")]
        public virtual AiAdoptionInitiatif AiAdoptionInitiatif
        {
            get
            {
                if (_aiAdoptionInitiatif == null && !string.IsNullOrEmpty(PrecedingInitiative))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiAdoptionInitiatif - no database context is set. PrecedingInitiative: " + PrecedingInitiative + ".");
                        }
                        return null;
                    }
                    _aiAdoptionInitiatif = base.SoAContext.AiAdoptionInitiatives.Find(PrecedingInitiative);
                    if (_aiAdoptionInitiatif != null)
                    {
                        base.SoAContext.Attach(_aiAdoptionInitiatif);
                    }
                }
                return _aiAdoptionInitiatif;
            }
            set
            {
                if (_aiAdoptionInitiatif != value)
                {
                    _aiAdoptionInitiatif = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_aiAdoptionInitiatif != null)
                    {
                        PrecedingInitiative = _aiAdoptionInitiatif.AiAdoptionInitiativeId;
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

        private ObservableCollection<AssistantAnswer> _assistantAnswers;

        [InverseProperty("AiAdoptionInitiatif")]
        public virtual ObservableCollection<AssistantAnswer> AssistantAnswers
        {
            get
            {
                if (_assistantAnswers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssistantAnswers - no database context is set. AiAdoptionInitiativeId: " + this.AiAdoptionInitiativeId + ".");
                        }
                        _assistantAnswers = new ObservableCollection<AssistantAnswer>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssistantAnswers.Where(x => x.ReviewedForInitiative == this.AiAdoptionInitiativeId).ToList<AssistantAnswer>();
                        _assistantAnswers = new ObservableCollection<AssistantAnswer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assistantAnswers.CollectionChanged += AssistantAnswers_CollectionChanged;
                }
                return _assistantAnswers;
            }
            private set
            {
                if (_assistantAnswers != null)
                {
                    _assistantAnswers.CollectionChanged -= AssistantAnswers_CollectionChanged;
                }
                _assistantAnswers = value;
                if (_assistantAnswers != null)
                {
                    _assistantAnswers.CollectionChanged += AssistantAnswers_CollectionChanged;
                }
            }
        }

        private void AssistantAnswers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssistantAnswer>())
                {
                    item.ReviewedForInitiative = this.AiAdoptionInitiativeId;
                }
            }
        }

        private ObservableCollection<AiAdoptionInitiatif> _aiAdoptionInitiatives;

        [InverseProperty("AiAdoptionInitiatif")]
        public virtual ObservableCollection<AiAdoptionInitiatif> AiAdoptionInitiatives
        {
            get
            {
                if (_aiAdoptionInitiatives == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiAdoptionInitiatives - no database context is set. AiAdoptionInitiativeId: " + this.AiAdoptionInitiativeId + ".");
                        }
                        _aiAdoptionInitiatives = new ObservableCollection<AiAdoptionInitiatif>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiAdoptionInitiatives.Where(x => x.PrecedingInitiative == this.AiAdoptionInitiativeId).ToList<AiAdoptionInitiatif>();
                        _aiAdoptionInitiatives = new ObservableCollection<AiAdoptionInitiatif>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiAdoptionInitiatives.CollectionChanged += AiAdoptionInitiatives_CollectionChanged;
                }
                return _aiAdoptionInitiatives;
            }
            private set
            {
                if (_aiAdoptionInitiatives != null)
                {
                    _aiAdoptionInitiatives.CollectionChanged -= AiAdoptionInitiatives_CollectionChanged;
                }
                _aiAdoptionInitiatives = value;
                if (_aiAdoptionInitiatives != null)
                {
                    _aiAdoptionInitiatives.CollectionChanged += AiAdoptionInitiatives_CollectionChanged;
                }
            }
        }

        private void AiAdoptionInitiatives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiAdoptionInitiatif>())
                {
                    item.PrecedingInitiative = this.AiAdoptionInitiativeId;
                }
            }
        }

        private ObservableCollection<KnowledgeOutcomeMeasurement> _knowledgeOutcomeMeasurements;

        [InverseProperty("AiAdoptionInitiatif")]
        public virtual ObservableCollection<KnowledgeOutcomeMeasurement> KnowledgeOutcomeMeasurements
        {
            get
            {
                if (_knowledgeOutcomeMeasurements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeOutcomeMeasurements - no database context is set. AiAdoptionInitiativeId: " + this.AiAdoptionInitiativeId + ".");
                        }
                        _knowledgeOutcomeMeasurements = new ObservableCollection<KnowledgeOutcomeMeasurement>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeOutcomeMeasurements.Where(x => x.AiInitiative == this.AiAdoptionInitiativeId).ToList<KnowledgeOutcomeMeasurement>();
                        _knowledgeOutcomeMeasurements = new ObservableCollection<KnowledgeOutcomeMeasurement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeOutcomeMeasurements.CollectionChanged += KnowledgeOutcomeMeasurements_CollectionChanged;
                }
                return _knowledgeOutcomeMeasurements;
            }
            private set
            {
                if (_knowledgeOutcomeMeasurements != null)
                {
                    _knowledgeOutcomeMeasurements.CollectionChanged -= KnowledgeOutcomeMeasurements_CollectionChanged;
                }
                _knowledgeOutcomeMeasurements = value;
                if (_knowledgeOutcomeMeasurements != null)
                {
                    _knowledgeOutcomeMeasurements.CollectionChanged += KnowledgeOutcomeMeasurements_CollectionChanged;
                }
            }
        }

        private void KnowledgeOutcomeMeasurements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeOutcomeMeasurement>())
                {
                    item.AiInitiative = this.AiAdoptionInitiativeId;
                }
            }
        }

        private ObservableCollection<AiInsightProposal> _aiInsightProposals;

        [InverseProperty("AiAdoptionInitiatif")]
        public virtual ObservableCollection<AiInsightProposal> AiInsightProposals
        {
            get
            {
                if (_aiInsightProposals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiInsightProposals - no database context is set. AiAdoptionInitiativeId: " + this.AiAdoptionInitiativeId + ".");
                        }
                        _aiInsightProposals = new ObservableCollection<AiInsightProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiInsightProposals.Where(x => x.SourceInitiative == this.AiAdoptionInitiativeId).ToList<AiInsightProposal>();
                        _aiInsightProposals = new ObservableCollection<AiInsightProposal>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiInsightProposals.CollectionChanged += AiInsightProposals_CollectionChanged;
                }
                return _aiInsightProposals;
            }
            private set
            {
                if (_aiInsightProposals != null)
                {
                    _aiInsightProposals.CollectionChanged -= AiInsightProposals_CollectionChanged;
                }
                _aiInsightProposals = value;
                if (_aiInsightProposals != null)
                {
                    _aiInsightProposals.CollectionChanged += AiInsightProposals_CollectionChanged;
                }
            }
        }

        private void AiInsightProposals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiInsightProposal>())
                {
                    item.SourceInitiative = this.AiAdoptionInitiativeId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.AgentRef;
            _ = this.Procedure;
            _ = this.ProcedureVersion;
            _ = this.AiAdoptionInitiatif;
            _ = this.EvaluationContextRef;
            _ = this.AssistantAnswers;
            _ = this.AiAdoptionInitiatives;
            _ = this.KnowledgeOutcomeMeasurements;
            _ = this.AiInsightProposals;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
