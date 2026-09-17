
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
    [Table("ProcedureStatusChanges")]
    public class ProcedureStatusChangeBase : SoAEntityBase
    {
        [Key]
        public string ProcedureStatusChangeId { get; set; }

        // Formula Name (rulebook: ={{ProcedureVersion}} & ": " & {{FromStatus}} & " -> " & {{ToStatus}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProcedureVersion)), F.S(": "), F.Text(F.Of(this.FromStatus)), F.S(" -> "), F.Text(F.Of(this.ToStatus))))); set { }
        }

        public DateTimeOffset? ChangedAt { get; set; }
        public string? Motivation { get; set; }
        public string? SemanticTypeIri { get; set; }
        public string? ChangeKind { get; set; }
        // Formula IsUnattributedChange (rulebook: ={{ChangedByAgent}} = "")
        [NotMapped]
        public bool? IsUnattributedChange
        {
            get => F.AsBool(F.Memo(this, "IsUnattributedChange", () => F.IsBlank(F.Of(this.ChangedByAgent)))); set { }
        }

        // Formula ChangeKindContradictsTarget (rulebook: =OR(AND({{ChangeKind}} = "Approve", {{ToStatus}} <> "Approved"), AND({{ChangeKind}} = "Validate", {{ToStatus}} <> "Validation"), AND({{ChangeKind}} = "Archive", {{ToStatus}} <> "Archived", {{ToStatus}} <> "Deprecated")))
        [NotMapped]
        public bool? ChangeKindContradictsTarget
        {
            get => F.AsBool(F.Memo(this, "ChangeKindContradictsTarget", () => F.Or(F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeKind)), F.S("Approve"))), F.Bool3(F.Ne(F.Nullif(F.Of(this.ToStatus)), F.S("Approved"))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeKind)), F.S("Validate"))), F.Bool3(F.Ne(F.Nullif(F.Of(this.ToStatus)), F.S("Validation"))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeKind)), F.S("Archive"))), F.Bool3(F.Ne(F.Nullif(F.Of(this.ToStatus)), F.S("Archived"))), F.Bool3(F.Ne(F.Nullif(F.Of(this.ToStatus)), F.S("Deprecated")))))))); set { }
        }


        public string? ProcedureVersion { get; set; }
        public string? FromStatus { get; set; }
        public string? ToStatus { get; set; }
        public string? ChangedByAgent { get; set; }
        public string? ProcedureExecution { get; set; }

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

        private LifecycleStatuse _lifecycleStatuse;

        [ForeignKey("FromStatus")]
        public virtual LifecycleStatuse LifecycleStatuse
        {
            get
            {
                if (_lifecycleStatuse == null && !string.IsNullOrEmpty(FromStatus))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LifecycleStatuse - no database context is set. FromStatus: " + FromStatus + ".");
                        }
                        return null;
                    }
                    _lifecycleStatuse = base.SoAContext.LifecycleStatuses.Find(FromStatus);
                    if (_lifecycleStatuse != null)
                    {
                        base.SoAContext.Attach(_lifecycleStatuse);
                    }
                }
                return _lifecycleStatuse;
            }
            set
            {
                if (_lifecycleStatuse != value)
                {
                    _lifecycleStatuse = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_lifecycleStatuse != null)
                    {
                        FromStatus = _lifecycleStatuse.LifecycleStatusId;
                    }
                }
            }
        }

        private LifecycleStatuse _lifecycleStatuseRef;

        [ForeignKey("ToStatus")]
        public virtual LifecycleStatuse LifecycleStatuseRef
        {
            get
            {
                if (_lifecycleStatuseRef == null && !string.IsNullOrEmpty(ToStatus))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LifecycleStatuseRef - no database context is set. ToStatus: " + ToStatus + ".");
                        }
                        return null;
                    }
                    _lifecycleStatuseRef = base.SoAContext.LifecycleStatuses.Find(ToStatus);
                    if (_lifecycleStatuseRef != null)
                    {
                        base.SoAContext.Attach(_lifecycleStatuseRef);
                    }
                }
                return _lifecycleStatuseRef;
            }
            set
            {
                if (_lifecycleStatuseRef != value)
                {
                    _lifecycleStatuseRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_lifecycleStatuseRef != null)
                    {
                        ToStatus = _lifecycleStatuseRef.LifecycleStatusId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("ChangedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ChangedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ChangedByAgent: " + ChangedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ChangedByAgent);
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
                        ChangedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private ProcedureExecution _procedureExecutionRef;

        [ForeignKey("ProcedureExecution")]
        public virtual ProcedureExecution ProcedureExecutionRef
        {
            get
            {
                if (_procedureExecutionRef == null && !string.IsNullOrEmpty(ProcedureExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecutionRef - no database context is set. ProcedureExecution: " + ProcedureExecution + ".");
                        }
                        return null;
                    }
                    _procedureExecutionRef = base.SoAContext.ProcedureExecutions.Find(ProcedureExecution);
                    if (_procedureExecutionRef != null)
                    {
                        base.SoAContext.Attach(_procedureExecutionRef);
                    }
                }
                return _procedureExecutionRef;
            }
            set
            {
                if (_procedureExecutionRef != value)
                {
                    _procedureExecutionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureExecutionRef != null)
                    {
                        ProcedureExecution = _procedureExecutionRef.ProcedureExecutionId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.LifecycleStatuse;
            _ = this.LifecycleStatuseRef;
            _ = this.Agent;
            _ = this.ProcedureExecutionRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
