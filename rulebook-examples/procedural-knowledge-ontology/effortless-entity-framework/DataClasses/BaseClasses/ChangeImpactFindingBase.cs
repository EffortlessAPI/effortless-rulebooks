
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
    [Table("ChangeImpactFindings")]
    public class ChangeImpactFindingBase : SoAEntityBase
    {
        [Key]
        public string ChangeImpactFindingId { get; set; }

        // Formula Name (rulebook: ={{ModelChangeRequest}} & " " & {{FindingKind}} & " " & {{FindingStage}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ModelChangeRequest)), F.S(" "), F.Text(F.Of(this.FindingKind)), F.S(" "), F.Text(F.Of(this.FindingStage))))); set { }
        }

        public string? FindingKind { get; set; }
        public string? FindingStage { get; set; }
        public string? AffectedRowKey { get; set; }
        public string? Description { get; set; }
        public string? DetectedByQuery { get; set; }
        public string? QueryLanguage { get; set; }
        public DateTimeOffset? FoundAt { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? ModelChangeRequest { get; set; }
        public string? AffectedTable { get; set; }
        public string? FoundByAgent { get; set; }

        private ModelChangeRequest _modelChangeRequestRef;

        [ForeignKey("ModelChangeRequest")]
        public virtual ModelChangeRequest ModelChangeRequestRef
        {
            get
            {
                if (_modelChangeRequestRef == null && !string.IsNullOrEmpty(ModelChangeRequest))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelChangeRequestRef - no database context is set. ModelChangeRequest: " + ModelChangeRequest + ".");
                        }
                        return null;
                    }
                    _modelChangeRequestRef = base.SoAContext.ModelChangeRequests.Find(ModelChangeRequest);
                    if (_modelChangeRequestRef != null)
                    {
                        base.SoAContext.Attach(_modelChangeRequestRef);
                    }
                }
                return _modelChangeRequestRef;
            }
            set
            {
                if (_modelChangeRequestRef != value)
                {
                    _modelChangeRequestRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_modelChangeRequestRef != null)
                    {
                        ModelChangeRequest = _modelChangeRequestRef.ModelChangeRequestId;
                    }
                }
            }
        }

        private RulebookTable _rulebookTable;

        [ForeignKey("AffectedTable")]
        public virtual RulebookTable RulebookTable
        {
            get
            {
                if (_rulebookTable == null && !string.IsNullOrEmpty(AffectedTable))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTable - no database context is set. AffectedTable: " + AffectedTable + ".");
                        }
                        return null;
                    }
                    _rulebookTable = base.SoAContext.RulebookTables.Find(AffectedTable);
                    if (_rulebookTable != null)
                    {
                        base.SoAContext.Attach(_rulebookTable);
                    }
                }
                return _rulebookTable;
            }
            set
            {
                if (_rulebookTable != value)
                {
                    _rulebookTable = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookTable != null)
                    {
                        AffectedTable = _rulebookTable.RulebookTableId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("FoundByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(FoundByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. FoundByAgent: " + FoundByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(FoundByAgent);
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
                        FoundByAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ModelChangeRequestRef;
            _ = this.RulebookTable;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
