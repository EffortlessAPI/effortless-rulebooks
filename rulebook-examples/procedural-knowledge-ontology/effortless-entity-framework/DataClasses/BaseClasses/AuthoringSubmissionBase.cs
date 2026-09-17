
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
    [Table("AuthoringSubmissions")]
    public class AuthoringSubmissionBase : SoAEntityBase
    {
        [Key]
        public string AuthoringSubmissionId { get; set; }

        // Formula Name (rulebook: ={{ProcedureVersion}} & " by " & {{SubmittedByAgent}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProcedureVersion)), F.S(" by "), F.Text(F.Of(this.SubmittedByAgent))))); set { }
        }

        public DateTimeOffset? SubmittedAt { get; set; }
        public string? SubmitterExpertise { get; set; }
        public int? StepCount { get; set; }
        public bool? ProfileValidationPassed { get; set; }
        public int? ValidationErrorCount { get; set; }
        public bool? WasAccepted { get; set; }
        // Formula IsExpertAuthoredConforming (rulebook: =AND({{SubmitterExpertise}} = "DomainExpert", {{ProfileValidationPassed}}))
        [NotMapped]
        public bool? IsExpertAuthoredConforming
        {
            get => F.AsBool(F.Memo(this, "IsExpertAuthoredConforming", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.SubmitterExpertise)), F.S("DomainExpert"))), F.IsTrueV(F.Of(this.ProfileValidationPassed))))); set { }
        }

        // Formula IsNonConformingAccepted (rulebook: =AND({{WasAccepted}}, {{ProfileValidationPassed}} = FALSE))
        [NotMapped]
        public bool? IsNonConformingAccepted
        {
            get => F.AsBool(F.Memo(this, "IsNonConformingAccepted", () => F.And(F.IsTrueV(F.Of(this.WasAccepted)), F.Bool3(F.Eq(F.Nullif(F.Of(this.ProfileValidationPassed)), F.B(false)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? SubmittedByAgent { get; set; }
        public string? AuthoringTool { get; set; }

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

        [ForeignKey("SubmittedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(SubmittedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. SubmittedByAgent: " + SubmittedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(SubmittedByAgent);
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
                        SubmittedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Tool _tool;

        [ForeignKey("AuthoringTool")]
        public virtual Tool Tool
        {
            get
            {
                if (_tool == null && !string.IsNullOrEmpty(AuthoringTool))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Tool - no database context is set. AuthoringTool: " + AuthoringTool + ".");
                        }
                        return null;
                    }
                    _tool = base.SoAContext.Tools.Find(AuthoringTool);
                    if (_tool != null)
                    {
                        base.SoAContext.Attach(_tool);
                    }
                }
                return _tool;
            }
            set
            {
                if (_tool != value)
                {
                    _tool = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_tool != null)
                    {
                        AuthoringTool = _tool.ToolId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.Agent;
            _ = this.Tool;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
