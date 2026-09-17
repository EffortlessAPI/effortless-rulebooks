
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
    [Table("AiInsightProposals")]
    public class AiInsightProposalBase : SoAEntityBase
    {
        [Key]
        public string AiInsightProposalId { get; set; }

        // Formula Name (rulebook: ={{ProposingAgent}} & ": " & LEFT({{Statement}}, 50))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProposingAgent)), F.S(": "), F.Text(F.Left(F.Of(this.Statement), F.I(50)))))); set { }
        }

        public string? InsightKind { get; set; }
        public string? Statement { get; set; }
        public DateTimeOffset? ProposedAt { get; set; }
        public string? ValidationVerdict { get; set; }
        // Formula TargetCreatorKind (rulebook: =INDEX(ProcedureVersions!{{CreatedByAgentKind}}, MATCH({{TargetProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public string? TargetCreatorKind
        {
            get => F.AsString(F.Memo(this, "TargetCreatorKind", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.TargetProcedureVersion), __r => F.Of(__r.CreatedByAgentKind), () => F.Of(new ProcedureVersion().CreatedByAgentKind)))); set { }
        }

        // Formula IsUnvalidatedOrStrandedInsight (rulebook: =OR(AND({{FoldedIntoChangeRequest}} <> "", {{ValidatedByAgent}} = ""), AND({{ValidationVerdict}} = "Valid", {{FoldedIntoChangeRequest}} = "")))
        [NotMapped]
        public bool? IsUnvalidatedOrStrandedInsight
        {
            get => F.AsBool(F.Memo(this, "IsUnvalidatedOrStrandedInsight", () => F.Or(F.Bool3(F.And(F.Bool3(F.IsNotBlank(F.Of(this.FoldedIntoChangeRequest))), F.Bool3(F.IsBlank(F.Of(this.ValidatedByAgent))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ValidationVerdict)), F.S("Valid"))), F.Bool3(F.IsBlank(F.Of(this.FoldedIntoChangeRequest)))))))); set { }
        }

        // Formula GrewModelWithoutHumanSeed (rulebook: =AND({{InsightKind}} = "ModelExtension", OR({{TargetProcedureVersion}} = "", {{TargetCreatorKind}} <> "Human")))
        [NotMapped]
        public bool? GrewModelWithoutHumanSeed
        {
            get => F.AsBool(F.Memo(this, "GrewModelWithoutHumanSeed", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.InsightKind)), F.S("ModelExtension"))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.TargetProcedureVersion))), F.Bool3(F.Ne(F.Of(this.TargetCreatorKind), F.S("Human")))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProposingAgent { get; set; }
        public string? SourceInitiative { get; set; }
        public string? TargetProcedureVersion { get; set; }
        public string? ValidatedByAgent { get; set; }
        public string? FoldedIntoChangeRequest { get; set; }

        private Agent _agent;

        [ForeignKey("ProposingAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ProposingAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ProposingAgent: " + ProposingAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ProposingAgent);
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
                        ProposingAgent = _agent.AgentId;
                    }
                }
            }
        }

        private AiAdoptionInitiatif _aiAdoptionInitiatif;

        [ForeignKey("SourceInitiative")]
        public virtual AiAdoptionInitiatif AiAdoptionInitiatif
        {
            get
            {
                if (_aiAdoptionInitiatif == null && !string.IsNullOrEmpty(SourceInitiative))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiAdoptionInitiatif - no database context is set. SourceInitiative: " + SourceInitiative + ".");
                        }
                        return null;
                    }
                    _aiAdoptionInitiatif = base.SoAContext.AiAdoptionInitiatives.Find(SourceInitiative);
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
                        SourceInitiative = _aiAdoptionInitiatif.AiAdoptionInitiativeId;
                    }
                }
            }
        }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("TargetProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(TargetProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. TargetProcedureVersion: " + TargetProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = base.SoAContext.ProcedureVersions.Find(TargetProcedureVersion);
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
                        TargetProcedureVersion = _procedureVersion.ProcedureVersionId;
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

        private ChangeRequest _changeRequest;

        [ForeignKey("FoldedIntoChangeRequest")]
        public virtual ChangeRequest ChangeRequest
        {
            get
            {
                if (_changeRequest == null && !string.IsNullOrEmpty(FoldedIntoChangeRequest))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeRequest - no database context is set. FoldedIntoChangeRequest: " + FoldedIntoChangeRequest + ".");
                        }
                        return null;
                    }
                    _changeRequest = base.SoAContext.ChangeRequests.Find(FoldedIntoChangeRequest);
                    if (_changeRequest != null)
                    {
                        base.SoAContext.Attach(_changeRequest);
                    }
                }
                return _changeRequest;
            }
            set
            {
                if (_changeRequest != value)
                {
                    _changeRequest = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_changeRequest != null)
                    {
                        FoldedIntoChangeRequest = _changeRequest.ChangeRequestId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Agent;
            _ = this.AiAdoptionInitiatif;
            _ = this.ProcedureVersion;
            _ = this.AgentRef;
            _ = this.ChangeRequest;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
