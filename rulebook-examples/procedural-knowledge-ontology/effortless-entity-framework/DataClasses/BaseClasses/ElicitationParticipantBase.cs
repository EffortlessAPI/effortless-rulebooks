
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
    [Table("ElicitationParticipants")]
    public class ElicitationParticipantBase : SoAEntityBase
    {
        [Key]
        public string ElicitationParticipantId { get; set; }

        // Formula Name (rulebook: ={{Agent}} & " in " & {{ElicitationSession}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Agent)), F.S(" in "), F.Text(F.Of(this.ElicitationSession))))); set { }
        }

        public string? ParticipationRole { get; set; }
        public string? KnowledgeFlow { get; set; }
        public string? ProcessStake { get; set; }
        public bool? IsUsuallyInvited { get; set; }
        // Formula IsPractitioner (rulebook: ={{ParticipationRole}} = "Practitioner")
        [NotMapped]
        public bool? IsPractitioner
        {
            get => F.AsBool(F.Memo(this, "IsPractitioner", () => F.Eq(F.Nullif(F.Of(this.ParticipationRole)), F.S("Practitioner")))); set { }
        }

        // Formula IsSubjectMatterExpert (rulebook: ={{ParticipationRole}} = "SubjectMatterExpert")
        [NotMapped]
        public bool? IsSubjectMatterExpert
        {
            get => F.AsBool(F.Memo(this, "IsSubjectMatterExpert", () => F.Eq(F.Nullif(F.Of(this.ParticipationRole)), F.S("SubjectMatterExpert")))); set { }
        }

        // Formula IsKnowledgeEngineer (rulebook: ={{ParticipationRole}} = "KnowledgeEngineer")
        [NotMapped]
        public bool? IsKnowledgeEngineer
        {
            get => F.AsBool(F.Memo(this, "IsKnowledgeEngineer", () => F.Eq(F.Nullif(F.Of(this.ParticipationRole)), F.S("KnowledgeEngineer")))); set { }
        }

        // Formula IsKnowledgeProducer (rulebook: =OR({{KnowledgeFlow}} = "Producer", {{KnowledgeFlow}} = "Both"))
        [NotMapped]
        public bool? IsKnowledgeProducer
        {
            get => F.AsBool(F.Memo(this, "IsKnowledgeProducer", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.KnowledgeFlow)), F.S("Producer"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.KnowledgeFlow)), F.S("Both")))))); set { }
        }

        // Formula IsKnowledgeConsumer (rulebook: =OR({{KnowledgeFlow}} = "Consumer", {{KnowledgeFlow}} = "Both"))
        [NotMapped]
        public bool? IsKnowledgeConsumer
        {
            get => F.AsBool(F.Memo(this, "IsKnowledgeConsumer", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.KnowledgeFlow)), F.S("Consumer"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.KnowledgeFlow)), F.S("Both")))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ElicitationSession { get; set; }
        public string? Agent { get; set; }

        private ElicitationSession _elicitationSessionRef;

        [ForeignKey("ElicitationSession")]
        public virtual ElicitationSession ElicitationSessionRef
        {
            get
            {
                if (_elicitationSessionRef == null && !string.IsNullOrEmpty(ElicitationSession))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ElicitationSessionRef - no database context is set. ElicitationSession: " + ElicitationSession + ".");
                        }
                        return null;
                    }
                    _elicitationSessionRef = base.SoAContext.ElicitationSessions.Find(ElicitationSession);
                    if (_elicitationSessionRef != null)
                    {
                        base.SoAContext.Attach(_elicitationSessionRef);
                    }
                }
                return _elicitationSessionRef;
            }
            set
            {
                if (_elicitationSessionRef != value)
                {
                    _elicitationSessionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_elicitationSessionRef != null)
                    {
                        ElicitationSession = _elicitationSessionRef.ElicitationSessionId;
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


        protected override void LazyLoadProperties()
        {
            _ = this.ElicitationSessionRef;
            _ = this.AgentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
