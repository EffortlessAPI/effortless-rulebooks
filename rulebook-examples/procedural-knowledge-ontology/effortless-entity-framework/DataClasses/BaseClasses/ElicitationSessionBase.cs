
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
    [Table("ElicitationSessions")]
    public class ElicitationSessionBase : SoAEntityBase
    {
        [Key]
        public string ElicitationSessionId { get; set; }

        // Formula Name (rulebook: ={{Method}} & " / " & {{StartedAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Method)), F.S(" / "), F.DatetimeText(F.Of(this.StartedAt))))); set { }
        }

        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }
        public string? Summary { get; set; }
        public string? Status { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula DaysSinceElicited (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{EndedAt}}, "days"))
        [NotMapped]
        public int? DaysSinceElicited
        {
            get => F.AsInt(F.Memo(this, "DaysSinceElicited", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.EndedAt), F.S("days"))))); set { }
        }

        // Formula IsSingleWitnessMethod (rulebook: =OR({{Method}} = "Shadowing", {{Method}} = "PractitionerInterview"))
        [NotMapped]
        public bool? IsSingleWitnessMethod
        {
            get => F.AsBool(F.Memo(this, "IsSingleWitnessMethod", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("Shadowing"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("PractitionerInterview")))))); set { }
        }

        // Formula PractitionerIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{PractitionerAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public bool? PractitionerIsStillEngaged
        {
            get => F.AsBool(F.Memo(this, "PractitionerIsStillEngaged", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.PractitionerAgent), __r => F.Of(__r.IsStillEngaged), () => F.Of(new Agent().IsStillEngaged)))); set { }
        }

        // Formula ValidFragmentsProduced (rulebook: =COUNTIFS(KnowledgeFragments!{{ValidFragmentSessionKey}}, {{ElicitationSessionId}}))
        [NotMapped]
        public decimal? ValidFragmentsProduced
        {
            get => F.AsDecimal(F.Memo(this, "ValidFragmentsProduced", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritField(F.Of(__r.ValidFragmentSessionKey), F.Of(this.ElicitationSessionId)))))); set { }
        }

        // Formula IsHighYieldSession (rulebook: ={{ValidFragmentsProduced}} >= 3)
        [NotMapped]
        public bool? IsHighYieldSession
        {
            get => F.AsBool(F.Memo(this, "IsHighYieldSession", () => F.Cmp(F.Of(this.ValidFragmentsProduced), ">=", F.I(3)))); set { }
        }

        // Formula IsConcentratedSingleWitness (rulebook: =AND({{IsSingleWitnessMethod}}, {{IsHighYieldSession}}))
        [NotMapped]
        public bool? IsConcentratedSingleWitness
        {
            get => F.AsBool(F.Memo(this, "IsConcentratedSingleWitness", () => F.And(F.Bool3(F.Of(this.IsSingleWitnessMethod)), F.Bool3(F.Of(this.IsHighYieldSession))))); set { }
        }

        // Formula IsStaleConcentratedWitness (rulebook: =AND({{IsConcentratedSingleWitness}}, {{DaysSinceElicited}} > 180))
        [NotMapped]
        public bool? IsStaleConcentratedWitness
        {
            get => F.AsBool(F.Memo(this, "IsStaleConcentratedWitness", () => F.And(F.Bool3(F.Of(this.IsConcentratedSingleWitness)), F.Bool3(F.Cmp(F.Of(this.DaysSinceElicited), ">", F.I(180)))))); set { }
        }

        // Formula ConcentratedSessionVersionKey (rulebook: =IF({{IsConcentratedSingleWitness}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? ConcentratedSessionVersionKey
        {
            get => F.AsString(F.Memo(this, "ConcentratedSessionVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsConcentratedSingleWitness))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        public string? ObserverStance { get; set; }
        public string? Setting { get; set; }
        public string? RecordingReference { get; set; }
        public DateTimeOffset? PractitionerReviewedAt { get; set; }
        // Formula ElicitationMode (rulebook: =IF({{Method}} = "PractitionerInterview", "Interview", IF({{Method}} = "Shadowing", "Observation", IF({{Method}} = "FacilitatedWorkshop", "Workshop", IF({{Method}} = "CriticalIncidentTechnique", "Incident", IF(OR({{Method}} = "ThinkAloudProtocol", {{Method}} = "RetrospectiveProtocol", {{Method}} = "ConceptLaddering", {{Method}} = "RepertoryGrid"), "Protocol", "Other"))))))
        [NotMapped]
        public string? ElicitationMode
        {
            get => F.AsString(F.Memo(this, "ElicitationMode", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("PractitionerInterview")))) ? F.S("Interview") : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("Shadowing")))) ? F.S("Observation") : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("FacilitatedWorkshop")))) ? F.S("Workshop") : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("CriticalIncidentTechnique")))) ? F.S("Incident") : (F.Truthy(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("ThinkAloudProtocol"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("RetrospectiveProtocol"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("ConceptLaddering"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("RepertoryGrid")))))) ? F.S("Protocol") : F.S("Other")))))))); set { }
        }

        // Formula IsInterview (rulebook: ={{ElicitationMode}} = "Interview")
        [NotMapped]
        public bool? IsInterview
        {
            get => F.AsBool(F.Memo(this, "IsInterview", () => F.Eq(F.Of(this.ElicitationMode), F.S("Interview")))); set { }
        }

        // Formula IsWorkshop (rulebook: ={{ElicitationMode}} = "Workshop")
        [NotMapped]
        public bool? IsWorkshop
        {
            get => F.AsBool(F.Memo(this, "IsWorkshop", () => F.Eq(F.Of(this.ElicitationMode), F.S("Workshop")))); set { }
        }

        // Formula WhyProbeCount (rulebook: =COUNTIFS(InterviewProbes!{{ElicitationSession}}, {{ElicitationSessionId}}, InterviewProbes!{{ProbeKind}}, "Why"))
        [NotMapped]
        public int? WhyProbeCount
        {
            get => F.AsInt(F.Memo(this, "WhyProbeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<InterviewProbe>(base.SoAContext, "InterviewProbes", __c => __c.InterviewProbes), __r => F.CritField(F.Of(__r.ElicitationSession), F.Of(this.ElicitationSessionId)) && F.CritLiteral(F.Of(__r.ProbeKind), F.S("Why"))))))); set { }
        }

        // Formula ShortfallProbeCount (rulebook: =COUNTIFS(InterviewProbes!{{ElicitationSession}}, {{ElicitationSessionId}}, InterviewProbes!{{ProbeKind}}, "ProcedureFallsShort"))
        [NotMapped]
        public int? ShortfallProbeCount
        {
            get => F.AsInt(F.Memo(this, "ShortfallProbeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<InterviewProbe>(base.SoAContext, "InterviewProbes", __c => __c.InterviewProbes), __r => F.CritField(F.Of(__r.ElicitationSession), F.Of(this.ElicitationSessionId)) && F.CritLiteral(F.Of(__r.ProbeKind), F.S("ProcedureFallsShort"))))))); set { }
        }

        // Formula IsInterviewWithoutWhyProbe (rulebook: =AND({{IsInterview}}, {{WhyProbeCount}} = 0))
        [NotMapped]
        public bool? IsInterviewWithoutWhyProbe
        {
            get => F.AsBool(F.Memo(this, "IsInterviewWithoutWhyProbe", () => F.And(F.Bool3(F.Of(this.IsInterview)), F.Bool3(F.Eq(F.Of(this.WhyProbeCount), F.I(0)))))); set { }
        }

        // Formula IsInterviewWithoutShortfallProbe (rulebook: =AND({{IsInterview}}, {{ShortfallProbeCount}} = 0))
        [NotMapped]
        public bool? IsInterviewWithoutShortfallProbe
        {
            get => F.AsBool(F.Memo(this, "IsInterviewWithoutShortfallProbe", () => F.And(F.Bool3(F.Of(this.IsInterview)), F.Bool3(F.Eq(F.Of(this.ShortfallProbeCount), F.I(0)))))); set { }
        }

        // Formula InitiatorCount (rulebook: =COUNTIFS(ElicitationParticipants!{{ElicitationSession}}, {{ElicitationSessionId}}, ElicitationParticipants!{{ProcessStake}}, "Initiates"))
        [NotMapped]
        public int? InitiatorCount
        {
            get => F.AsInt(F.Memo(this, "InitiatorCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationParticipant>(base.SoAContext, "ElicitationParticipants", __c => __c.ElicitationParticipants), __r => F.CritField(F.Of(__r.ElicitationSession), F.Of(this.ElicitationSessionId)) && F.CritLiteral(F.Of(__r.ProcessStake), F.S("Initiates"))))))); set { }
        }

        // Formula ExecutorCount (rulebook: =COUNTIFS(ElicitationParticipants!{{ElicitationSession}}, {{ElicitationSessionId}}, ElicitationParticipants!{{ProcessStake}}, "Executes"))
        [NotMapped]
        public int? ExecutorCount
        {
            get => F.AsInt(F.Memo(this, "ExecutorCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationParticipant>(base.SoAContext, "ElicitationParticipants", __c => __c.ElicitationParticipants), __r => F.CritField(F.Of(__r.ElicitationSession), F.Of(this.ElicitationSessionId)) && F.CritLiteral(F.Of(__r.ProcessStake), F.S("Executes"))))))); set { }
        }

        // Formula DependentCount (rulebook: =COUNTIFS(ElicitationParticipants!{{ElicitationSession}}, {{ElicitationSessionId}}, ElicitationParticipants!{{ProcessStake}}, "DependsOnResults"))
        [NotMapped]
        public int? DependentCount
        {
            get => F.AsInt(F.Memo(this, "DependentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationParticipant>(base.SoAContext, "ElicitationParticipants", __c => __c.ElicitationParticipants), __r => F.CritField(F.Of(__r.ElicitationSession), F.Of(this.ElicitationSessionId)) && F.CritLiteral(F.Of(__r.ProcessStake), F.S("DependsOnResults"))))))); set { }
        }

        // Formula UninvitedParticipantCount (rulebook: =COUNTIFS(ElicitationParticipants!{{ElicitationSession}}, {{ElicitationSessionId}}, ElicitationParticipants!{{IsUsuallyInvited}}, FALSE))
        [NotMapped]
        public int? UninvitedParticipantCount
        {
            get => F.AsInt(F.Memo(this, "UninvitedParticipantCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationParticipant>(base.SoAContext, "ElicitationParticipants", __c => __c.ElicitationParticipants), __r => F.CritField(F.Of(__r.ElicitationSession), F.Of(this.ElicitationSessionId)) && F.CritLiteral(F.Of(__r.IsUsuallyInvited), F.B(false))))))); set { }
        }

        // Formula GathersWholeProcessChain (rulebook: =AND({{IsWorkshop}}, {{InitiatorCount}} > 0, {{ExecutorCount}} > 0, {{DependentCount}} > 0))
        [NotMapped]
        public bool? GathersWholeProcessChain
        {
            get => F.AsBool(F.Memo(this, "GathersWholeProcessChain", () => F.And(F.Bool3(F.Of(this.IsWorkshop)), F.Bool3(F.Cmp(F.Of(this.InitiatorCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.ExecutorCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.DependentCount), ">", F.I(0)))))); set { }
        }

        // Formula IsWorkshopWithoutUsualOutsiders (rulebook: =AND({{IsWorkshop}}, {{UninvitedParticipantCount}} = 0))
        [NotMapped]
        public bool? IsWorkshopWithoutUsualOutsiders
        {
            get => F.AsBool(F.Memo(this, "IsWorkshopWithoutUsualOutsiders", () => F.And(F.Bool3(F.Of(this.IsWorkshop)), F.Bool3(F.Eq(F.Of(this.UninvitedParticipantCount), F.I(0)))))); set { }
        }

        // Formula MethodFamily (rulebook: =INDEX(KnowledgeMethods!{{MethodFamily}}, MATCH({{Method}}, KnowledgeMethods!{{KnowledgeMethodId}}, 0)))
        [NotMapped]
        public string? MethodFamily
        {
            get => F.AsString(F.Memo(this, "MethodFamily", () => F.Lookup<KnowledgeMethod>(this, "KnowledgeMethods", "KnowledgeMethodId", __c => __c.KnowledgeMethods, __r => F.Of(__r.KnowledgeMethodId), F.Of(this.Method), __r => F.Of(__r.MethodFamily), () => F.Of(new KnowledgeMethod().MethodFamily)))); set { }
        }

        // Formula FacilitatorKnowledgeEngineerRoleCount (rulebook: =COUNTIFS(Roles!{{CurrentAgent}}, {{FacilitatorAgent}}, Roles!{{RoleId}}, "knowledge-engineer"))
        [NotMapped]
        public int? FacilitatorKnowledgeEngineerRoleCount
        {
            get => F.AsInt(F.Memo(this, "FacilitatorKnowledgeEngineerRoleCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Role>(base.SoAContext, "Roles", __c => __c.Roles), __r => F.CritField(F.Of(__r.CurrentAgent), F.Of(this.FacilitatorAgent)) && F.CritLiteral(F.Of(__r.RoleId), F.S("knowledge-engineer"))))))); set { }
        }

        // Formula FacilitatorIsKnowledgeEngineer (rulebook: ={{FacilitatorKnowledgeEngineerRoleCount}} > 0)
        [NotMapped]
        public bool? FacilitatorIsKnowledgeEngineer
        {
            get => F.AsBool(F.Memo(this, "FacilitatorIsKnowledgeEngineer", () => F.Cmp(F.Of(this.FacilitatorKnowledgeEngineerRoleCount), ">", F.I(0)))); set { }
        }

        // Formula IsGenericOrUnskilledCapture (rulebook: =AND({{Method}} <> "", OR({{MethodFamily}} <> "Elicitation", NOT({{FacilitatorIsKnowledgeEngineer}}))))
        [NotMapped]
        public bool? IsGenericOrUnskilledCapture
        {
            get => F.AsBool(F.Memo(this, "IsGenericOrUnskilledCapture", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.Method))), F.Bool3(F.Or(F.Bool3(F.Ne(F.Of(this.MethodFamily), F.S("Elicitation"))), F.Bool3(F.Not(F.Bool3(F.Of(this.FacilitatorIsKnowledgeEngineer))))))))); set { }
        }

        // Formula IsKeFieldSession (rulebook: =AND({{FacilitatorIsKnowledgeEngineer}}, {{Setting}} = "InSitu"))
        [NotMapped]
        public bool? IsKeFieldSession
        {
            get => F.AsBool(F.Memo(this, "IsKeFieldSession", () => F.And(F.Bool3(F.Of(this.FacilitatorIsKnowledgeEngineer)), F.Bool3(F.Eq(F.Nullif(F.Of(this.Setting)), F.S("InSitu")))))); set { }
        }

        // Formula IsReviewedRecording (rulebook: =AND({{RecordingReference}} <> "", {{PractitionerReviewedAt}} <> ""))
        [NotMapped]
        public bool? IsReviewedRecording
        {
            get => F.AsBool(F.Memo(this, "IsReviewedRecording", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.RecordingReference))), F.Bool3(F.IsNotBlank(F.Of(this.PractitionerReviewedAt)))))); set { }
        }


        public string? ProcedureVersion { get; set; }
        public string? Method { get; set; }
        public string? PractitionerAgent { get; set; }
        public string? FacilitatorAgent { get; set; }
        public string? EvaluationContext { get; set; }

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

        private KnowledgeMethod _knowledgeMethod;

        [ForeignKey("Method")]
        public virtual KnowledgeMethod KnowledgeMethod
        {
            get
            {
                if (_knowledgeMethod == null && !string.IsNullOrEmpty(Method))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeMethod - no database context is set. Method: " + Method + ".");
                        }
                        return null;
                    }
                    _knowledgeMethod = base.SoAContext.KnowledgeMethods.Find(Method);
                    if (_knowledgeMethod != null)
                    {
                        base.SoAContext.Attach(_knowledgeMethod);
                    }
                }
                return _knowledgeMethod;
            }
            set
            {
                if (_knowledgeMethod != value)
                {
                    _knowledgeMethod = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeMethod != null)
                    {
                        Method = _knowledgeMethod.KnowledgeMethodId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("PractitionerAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(PractitionerAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. PractitionerAgent: " + PractitionerAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(PractitionerAgent);
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
                        PractitionerAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("FacilitatorAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(FacilitatorAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. FacilitatorAgent: " + FacilitatorAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(FacilitatorAgent);
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
                        FacilitatorAgent = _agentRef.AgentId;
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

        private ObservableCollection<KnowledgeFragment> _knowledgeFragments;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<KnowledgeFragment> KnowledgeFragments
        {
            get
            {
                if (_knowledgeFragments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeFragments.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<KnowledgeFragment>();
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
                return _knowledgeFragments;
            }
            private set
            {
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged -= KnowledgeFragments_CollectionChanged;
                }
                _knowledgeFragments = value;
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
            }
        }

        private void KnowledgeFragments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeFragment>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<KnowledgeGap> _knowledgeGaps;

        [InverseProperty("ElicitationSession")]
        public virtual ObservableCollection<KnowledgeGap> KnowledgeGaps
        {
            get
            {
                if (_knowledgeGaps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeGaps - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeGaps.Where(x => x.DrawnOutBySession == this.ElicitationSessionId).ToList<KnowledgeGap>();
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeGaps.CollectionChanged += KnowledgeGaps_CollectionChanged;
                }
                return _knowledgeGaps;
            }
            private set
            {
                if (_knowledgeGaps != null)
                {
                    _knowledgeGaps.CollectionChanged -= KnowledgeGaps_CollectionChanged;
                }
                _knowledgeGaps = value;
                if (_knowledgeGaps != null)
                {
                    _knowledgeGaps.CollectionChanged += KnowledgeGaps_CollectionChanged;
                }
            }
        }

        private void KnowledgeGaps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeGap>())
                {
                    item.DrawnOutBySession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<PractitionerExpertise> _practitionerExpertise;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<PractitionerExpertise> PractitionerExpertise
        {
            get
            {
                if (_practitionerExpertise == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PractitionerExpertise - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _practitionerExpertise = new ObservableCollection<PractitionerExpertise>();
                    }
                    else
                    {
                        var items = base.SoAContext.PractitionerExpertise.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<PractitionerExpertise>();
                        _practitionerExpertise = new ObservableCollection<PractitionerExpertise>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _practitionerExpertise.CollectionChanged += PractitionerExpertise_CollectionChanged;
                }
                return _practitionerExpertise;
            }
            private set
            {
                if (_practitionerExpertise != null)
                {
                    _practitionerExpertise.CollectionChanged -= PractitionerExpertise_CollectionChanged;
                }
                _practitionerExpertise = value;
                if (_practitionerExpertise != null)
                {
                    _practitionerExpertise.CollectionChanged += PractitionerExpertise_CollectionChanged;
                }
            }
        }

        private void PractitionerExpertise_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<PractitionerExpertise>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<CriticalIncident> _criticalIncidents;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<CriticalIncident> CriticalIncidents
        {
            get
            {
                if (_criticalIncidents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CriticalIncidents - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _criticalIncidents = new ObservableCollection<CriticalIncident>();
                    }
                    else
                    {
                        var items = base.SoAContext.CriticalIncidents.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<CriticalIncident>();
                        _criticalIncidents = new ObservableCollection<CriticalIncident>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _criticalIncidents.CollectionChanged += CriticalIncidents_CollectionChanged;
                }
                return _criticalIncidents;
            }
            private set
            {
                if (_criticalIncidents != null)
                {
                    _criticalIncidents.CollectionChanged -= CriticalIncidents_CollectionChanged;
                }
                _criticalIncidents = value;
                if (_criticalIncidents != null)
                {
                    _criticalIncidents.CollectionChanged += CriticalIncidents_CollectionChanged;
                }
            }
        }

        private void CriticalIncidents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CriticalIncident>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<InterviewProbe> _interviewProbes;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<InterviewProbe> InterviewProbes
        {
            get
            {
                if (_interviewProbes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access InterviewProbes - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _interviewProbes = new ObservableCollection<InterviewProbe>();
                    }
                    else
                    {
                        var items = base.SoAContext.InterviewProbes.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<InterviewProbe>();
                        _interviewProbes = new ObservableCollection<InterviewProbe>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _interviewProbes.CollectionChanged += InterviewProbes_CollectionChanged;
                }
                return _interviewProbes;
            }
            private set
            {
                if (_interviewProbes != null)
                {
                    _interviewProbes.CollectionChanged -= InterviewProbes_CollectionChanged;
                }
                _interviewProbes = value;
                if (_interviewProbes != null)
                {
                    _interviewProbes.CollectionChanged += InterviewProbes_CollectionChanged;
                }
            }
        }

        private void InterviewProbes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<InterviewProbe>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<ObservedAction> _observedActions;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<ObservedAction> ObservedActions
        {
            get
            {
                if (_observedActions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ObservedActions - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _observedActions = new ObservableCollection<ObservedAction>();
                    }
                    else
                    {
                        var items = base.SoAContext.ObservedActions.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<ObservedAction>();
                        _observedActions = new ObservableCollection<ObservedAction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _observedActions.CollectionChanged += ObservedActions_CollectionChanged;
                }
                return _observedActions;
            }
            private set
            {
                if (_observedActions != null)
                {
                    _observedActions.CollectionChanged -= ObservedActions_CollectionChanged;
                }
                _observedActions = value;
                if (_observedActions != null)
                {
                    _observedActions.CollectionChanged += ObservedActions_CollectionChanged;
                }
            }
        }

        private void ObservedActions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ObservedAction>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<ElicitationParticipant> _elicitationParticipants;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<ElicitationParticipant> ElicitationParticipants
        {
            get
            {
                if (_elicitationParticipants == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ElicitationParticipants - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _elicitationParticipants = new ObservableCollection<ElicitationParticipant>();
                    }
                    else
                    {
                        var items = base.SoAContext.ElicitationParticipants.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<ElicitationParticipant>();
                        _elicitationParticipants = new ObservableCollection<ElicitationParticipant>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _elicitationParticipants.CollectionChanged += ElicitationParticipants_CollectionChanged;
                }
                return _elicitationParticipants;
            }
            private set
            {
                if (_elicitationParticipants != null)
                {
                    _elicitationParticipants.CollectionChanged -= ElicitationParticipants_CollectionChanged;
                }
                _elicitationParticipants = value;
                if (_elicitationParticipants != null)
                {
                    _elicitationParticipants.CollectionChanged += ElicitationParticipants_CollectionChanged;
                }
            }
        }

        private void ElicitationParticipants_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ElicitationParticipant>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<WorkflowViewDivergence> _workflowViewDivergences;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<WorkflowViewDivergence> WorkflowViewDivergences
        {
            get
            {
                if (_workflowViewDivergences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowViewDivergences - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _workflowViewDivergences = new ObservableCollection<WorkflowViewDivergence>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowViewDivergences.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<WorkflowViewDivergence>();
                        _workflowViewDivergences = new ObservableCollection<WorkflowViewDivergence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _workflowViewDivergences.CollectionChanged += WorkflowViewDivergences_CollectionChanged;
                }
                return _workflowViewDivergences;
            }
            private set
            {
                if (_workflowViewDivergences != null)
                {
                    _workflowViewDivergences.CollectionChanged -= WorkflowViewDivergences_CollectionChanged;
                }
                _workflowViewDivergences = value;
                if (_workflowViewDivergences != null)
                {
                    _workflowViewDivergences.CollectionChanged += WorkflowViewDivergences_CollectionChanged;
                }
            }
        }

        private void WorkflowViewDivergences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowViewDivergence>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<ExpertCognition> _expertCognitions;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<ExpertCognition> ExpertCognitions
        {
            get
            {
                if (_expertCognitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExpertCognitions - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _expertCognitions = new ObservableCollection<ExpertCognition>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExpertCognitions.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<ExpertCognition>();
                        _expertCognitions = new ObservableCollection<ExpertCognition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _expertCognitions.CollectionChanged += ExpertCognitions_CollectionChanged;
                }
                return _expertCognitions;
            }
            private set
            {
                if (_expertCognitions != null)
                {
                    _expertCognitions.CollectionChanged -= ExpertCognitions_CollectionChanged;
                }
                _expertCognitions = value;
                if (_expertCognitions != null)
                {
                    _expertCognitions.CollectionChanged += ExpertCognitions_CollectionChanged;
                }
            }
        }

        private void ExpertCognitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExpertCognition>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<ConceptLadderRung> _conceptLadderRungs;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<ConceptLadderRung> ConceptLadderRungs
        {
            get
            {
                if (_conceptLadderRungs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConceptLadderRungs - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _conceptLadderRungs = new ObservableCollection<ConceptLadderRung>();
                    }
                    else
                    {
                        var items = base.SoAContext.ConceptLadderRungs.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<ConceptLadderRung>();
                        _conceptLadderRungs = new ObservableCollection<ConceptLadderRung>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _conceptLadderRungs.CollectionChanged += ConceptLadderRungs_CollectionChanged;
                }
                return _conceptLadderRungs;
            }
            private set
            {
                if (_conceptLadderRungs != null)
                {
                    _conceptLadderRungs.CollectionChanged -= ConceptLadderRungs_CollectionChanged;
                }
                _conceptLadderRungs = value;
                if (_conceptLadderRungs != null)
                {
                    _conceptLadderRungs.CollectionChanged += ConceptLadderRungs_CollectionChanged;
                }
            }
        }

        private void ConceptLadderRungs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ConceptLadderRung>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<RepertoryGridConstruct> _repertoryGridConstructs;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<RepertoryGridConstruct> RepertoryGridConstructs
        {
            get
            {
                if (_repertoryGridConstructs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RepertoryGridConstructs - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _repertoryGridConstructs = new ObservableCollection<RepertoryGridConstruct>();
                    }
                    else
                    {
                        var items = base.SoAContext.RepertoryGridConstructs.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<RepertoryGridConstruct>();
                        _repertoryGridConstructs = new ObservableCollection<RepertoryGridConstruct>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _repertoryGridConstructs.CollectionChanged += RepertoryGridConstructs_CollectionChanged;
                }
                return _repertoryGridConstructs;
            }
            private set
            {
                if (_repertoryGridConstructs != null)
                {
                    _repertoryGridConstructs.CollectionChanged -= RepertoryGridConstructs_CollectionChanged;
                }
                _repertoryGridConstructs = value;
                if (_repertoryGridConstructs != null)
                {
                    _repertoryGridConstructs.CollectionChanged += RepertoryGridConstructs_CollectionChanged;
                }
            }
        }

        private void RepertoryGridConstructs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RepertoryGridConstruct>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<KnowledgeConversion> _knowledgeConversions;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<KnowledgeConversion> KnowledgeConversions
        {
            get
            {
                if (_knowledgeConversions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeConversions - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _knowledgeConversions = new ObservableCollection<KnowledgeConversion>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeConversions.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<KnowledgeConversion>();
                        _knowledgeConversions = new ObservableCollection<KnowledgeConversion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeConversions.CollectionChanged += KnowledgeConversions_CollectionChanged;
                }
                return _knowledgeConversions;
            }
            private set
            {
                if (_knowledgeConversions != null)
                {
                    _knowledgeConversions.CollectionChanged -= KnowledgeConversions_CollectionChanged;
                }
                _knowledgeConversions = value;
                if (_knowledgeConversions != null)
                {
                    _knowledgeConversions.CollectionChanged += KnowledgeConversions_CollectionChanged;
                }
            }
        }

        private void KnowledgeConversions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeConversion>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }

        private ObservableCollection<FragmentCorroboration> _fragmentCorroborations;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<FragmentCorroboration> FragmentCorroborations
        {
            get
            {
                if (_fragmentCorroborations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FragmentCorroborations - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _fragmentCorroborations = new ObservableCollection<FragmentCorroboration>();
                    }
                    else
                    {
                        var items = base.SoAContext.FragmentCorroborations.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<FragmentCorroboration>();
                        _fragmentCorroborations = new ObservableCollection<FragmentCorroboration>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fragmentCorroborations.CollectionChanged += FragmentCorroborations_CollectionChanged;
                }
                return _fragmentCorroborations;
            }
            private set
            {
                if (_fragmentCorroborations != null)
                {
                    _fragmentCorroborations.CollectionChanged -= FragmentCorroborations_CollectionChanged;
                }
                _fragmentCorroborations = value;
                if (_fragmentCorroborations != null)
                {
                    _fragmentCorroborations.CollectionChanged += FragmentCorroborations_CollectionChanged;
                }
            }
        }

        private void FragmentCorroborations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FragmentCorroboration>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.KnowledgeMethod;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.EvaluationContextRef;
            _ = this.KnowledgeFragments;
            _ = this.KnowledgeGaps;
            _ = this.PractitionerExpertise;
            _ = this.CriticalIncidents;
            _ = this.InterviewProbes;
            _ = this.ObservedActions;
            _ = this.ElicitationParticipants;
            _ = this.WorkflowViewDivergences;
            _ = this.ExpertCognitions;
            _ = this.ConceptLadderRungs;
            _ = this.RepertoryGridConstructs;
            _ = this.KnowledgeConversions;
            _ = this.FragmentCorroborations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
