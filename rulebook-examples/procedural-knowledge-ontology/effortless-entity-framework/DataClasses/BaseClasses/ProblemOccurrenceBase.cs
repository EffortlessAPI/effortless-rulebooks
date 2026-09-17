
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
    [Table("ProblemOccurrences")]
    public class ProblemOccurrenceBase : SoAEntityBase
    {
        [Key]
        public string ProblemOccurrenceId { get; set; }

        // Formula Name (rulebook: ={{ProblemSignature}} & " / " & {{ProblemOccurrenceId}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProblemSignature)), F.S(" / "), F.Text(F.Of(this.ProblemOccurrenceId))))); set { }
        }

        public string? ProblemSignature { get; set; }
        public DateTimeOffset? OccurredAt { get; set; }
        public DateTimeOffset? SolvedAt { get; set; }
        // Formula SolverIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{SolvedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public bool? SolverIsStillEngaged
        {
            get => F.AsBool(F.Memo(this, "SolverIsStillEngaged", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.SolvedByAgent), __r => F.Of(__r.IsStillEngaged), () => F.Of(new Agent().IsStillEngaged)))); set { }
        }

        // Formula IsSolved (rulebook: ={{SolvedAt}} <> "")
        [NotMapped]
        public bool? IsSolved
        {
            get => F.AsBool(F.Memo(this, "IsSolved", () => F.IsNotBlank(F.Of(this.SolvedAt)))); set { }
        }

        // Formula PriorWasSolved (rulebook: =INDEX(ProblemOccurrences!{{IsSolved}}, MATCH({{PriorOccurrence}}, ProblemOccurrences!{{ProblemOccurrenceId}}, 0)))
        [NotMapped]
        public bool? PriorWasSolved
        {
            get => F.AsBool(F.Memo(this, "PriorWasSolved", () => F.Lookup<ProblemOccurrence>(this, "ProblemOccurrences", "ProblemOccurrenceId", __c => __c.ProblemOccurrences, __r => F.Of(__r.ProblemOccurrenceId), F.Of(this.PriorOccurrence), __r => F.Of(__r.IsSolved), () => F.Of(new ProblemOccurrence().IsSolved)))); set { }
        }

        // Formula PriorSolutionEntry (rulebook: =INDEX(ProblemOccurrences!{{SolutionEntry}}, MATCH({{PriorOccurrence}}, ProblemOccurrences!{{ProblemOccurrenceId}}, 0)))
        [NotMapped]
        public string? PriorSolutionEntry
        {
            get => F.AsString(F.Memo(this, "PriorSolutionEntry", () => F.Lookup<ProblemOccurrence>(this, "ProblemOccurrences", "ProblemOccurrenceId", __c => __c.ProblemOccurrences, __r => F.Of(__r.ProblemOccurrenceId), F.Of(this.PriorOccurrence), __r => F.Of(__r.SolutionEntry), () => F.Of(new ProblemOccurrence().SolutionEntry)))); set { }
        }

        // Formula PriorSolver (rulebook: =INDEX(ProblemOccurrences!{{SolvedByAgent}}, MATCH({{PriorOccurrence}}, ProblemOccurrences!{{ProblemOccurrenceId}}, 0)))
        [NotMapped]
        public string? PriorSolver
        {
            get => F.AsString(F.Memo(this, "PriorSolver", () => F.Lookup<ProblemOccurrence>(this, "ProblemOccurrences", "ProblemOccurrenceId", __c => __c.ProblemOccurrences, __r => F.Of(__r.ProblemOccurrenceId), F.Of(this.PriorOccurrence), __r => F.Of(__r.SolvedByAgent), () => F.Of(new ProblemOccurrence().SolvedByAgent)))); set { }
        }

        // Formula PriorSolverIsStillEngaged (rulebook: =INDEX(ProblemOccurrences!{{SolverIsStillEngaged}}, MATCH({{PriorOccurrence}}, ProblemOccurrences!{{ProblemOccurrenceId}}, 0)))
        [NotMapped]
        public bool? PriorSolverIsStillEngaged
        {
            get => F.AsBool(F.Memo(this, "PriorSolverIsStillEngaged", () => F.Lookup<ProblemOccurrence>(this, "ProblemOccurrences", "ProblemOccurrenceId", __c => __c.ProblemOccurrences, __r => F.Of(__r.ProblemOccurrenceId), F.Of(this.PriorOccurrence), __r => F.Of(__r.SolverIsStillEngaged), () => F.Of(new ProblemOccurrence().SolverIsStillEngaged)))); set { }
        }

        // Formula IsSolvedWithoutRecordedSolution (rulebook: =AND({{IsSolved}}, {{SolutionEntry}} = ""))
        [NotMapped]
        public bool? IsSolvedWithoutRecordedSolution
        {
            get => F.AsBool(F.Memo(this, "IsSolvedWithoutRecordedSolution", () => F.And(F.Bool3(F.Of(this.IsSolved)), F.Bool3(F.IsBlank(F.Of(this.SolutionEntry)))))); set { }
        }

        // Formula HasBeenSolvedBefore (rulebook: =AND({{PriorOccurrence}} <> "", {{PriorWasSolved}} = TRUE))
        [NotMapped]
        public bool? HasBeenSolvedBefore
        {
            get => F.AsBool(F.Memo(this, "HasBeenSolvedBefore", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PriorOccurrence))), F.Bool3(F.Eq(F.Of(this.PriorWasSolved), F.B(true)))))); set { }
        }

        // Formula HasConsultableSpecialist (rulebook: =AND({{HasBeenSolvedBefore}}, {{PriorSolver}} <> "", {{PriorSolverIsStillEngaged}} = TRUE))
        [NotMapped]
        public bool? HasConsultableSpecialist
        {
            get => F.AsBool(F.Memo(this, "HasConsultableSpecialist", () => F.And(F.Bool3(F.Of(this.HasBeenSolvedBefore)), F.Bool3(F.IsNotBlank(F.Of(this.PriorSolver))), F.Bool3(F.Eq(F.Of(this.PriorSolverIsStillEngaged), F.B(true)))))); set { }
        }

        // Formula IsRelearnedSolvedProblem (rulebook: =AND({{HasBeenSolvedBefore}}, {{PriorSolutionEntry}} = ""))
        [NotMapped]
        public bool? IsRelearnedSolvedProblem
        {
            get => F.AsBool(F.Memo(this, "IsRelearnedSolvedProblem", () => F.And(F.Bool3(F.Of(this.HasBeenSolvedBefore)), F.Bool3(F.IsBlank(F.Of(this.PriorSolutionEntry)))))); set { }
        }

        // Formula IsTurnoverRegression (rulebook: =AND({{IsRelearnedSolvedProblem}}, {{PriorSolver}} <> "", {{PriorSolverIsStillEngaged}} = FALSE))
        [NotMapped]
        public bool? IsTurnoverRegression
        {
            get => F.AsBool(F.Memo(this, "IsTurnoverRegression", () => F.And(F.Bool3(F.Of(this.IsRelearnedSolvedProblem)), F.Bool3(F.IsNotBlank(F.Of(this.PriorSolver))), F.Bool3(F.Eq(F.Of(this.PriorSolverIsStillEngaged), F.B(false)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
        public string? SolvedByAgent { get; set; }
        public string? SolutionEntry { get; set; }
        public string? PriorOccurrence { get; set; }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("SolvedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(SolvedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. SolvedByAgent: " + SolvedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(SolvedByAgent);
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
                        SolvedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private KnowledgeRepositoryEntry _knowledgeRepositoryEntry;

        [ForeignKey("SolutionEntry")]
        public virtual KnowledgeRepositoryEntry KnowledgeRepositoryEntry
        {
            get
            {
                if (_knowledgeRepositoryEntry == null && !string.IsNullOrEmpty(SolutionEntry))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeRepositoryEntry - no database context is set. SolutionEntry: " + SolutionEntry + ".");
                        }
                        return null;
                    }
                    _knowledgeRepositoryEntry = base.SoAContext.KnowledgeRepositoryEntries.Find(SolutionEntry);
                    if (_knowledgeRepositoryEntry != null)
                    {
                        base.SoAContext.Attach(_knowledgeRepositoryEntry);
                    }
                }
                return _knowledgeRepositoryEntry;
            }
            set
            {
                if (_knowledgeRepositoryEntry != value)
                {
                    _knowledgeRepositoryEntry = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeRepositoryEntry != null)
                    {
                        SolutionEntry = _knowledgeRepositoryEntry.KnowledgeRepositoryEntryId;
                    }
                }
            }
        }

        private ProblemOccurrence _problemOccurrence;

        [ForeignKey("PriorOccurrence")]
        public virtual ProblemOccurrence ProblemOccurrence
        {
            get
            {
                if (_problemOccurrence == null && !string.IsNullOrEmpty(PriorOccurrence))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProblemOccurrence - no database context is set. PriorOccurrence: " + PriorOccurrence + ".");
                        }
                        return null;
                    }
                    _problemOccurrence = base.SoAContext.ProblemOccurrences.Find(PriorOccurrence);
                    if (_problemOccurrence != null)
                    {
                        base.SoAContext.Attach(_problemOccurrence);
                    }
                }
                return _problemOccurrence;
            }
            set
            {
                if (_problemOccurrence != value)
                {
                    _problemOccurrence = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_problemOccurrence != null)
                    {
                        PriorOccurrence = _problemOccurrence.ProblemOccurrenceId;
                    }
                }
            }
        }

        private ObservableCollection<ProblemOccurrence> _problemOccurrences;

        [InverseProperty("ProblemOccurrence")]
        public virtual ObservableCollection<ProblemOccurrence> ProblemOccurrences
        {
            get
            {
                if (_problemOccurrences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProblemOccurrences - no database context is set. ProblemOccurrenceId: " + this.ProblemOccurrenceId + ".");
                        }
                        _problemOccurrences = new ObservableCollection<ProblemOccurrence>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProblemOccurrences.Where(x => x.PriorOccurrence == this.ProblemOccurrenceId).ToList<ProblemOccurrence>();
                        _problemOccurrences = new ObservableCollection<ProblemOccurrence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _problemOccurrences.CollectionChanged += ProblemOccurrences_CollectionChanged;
                }
                return _problemOccurrences;
            }
            private set
            {
                if (_problemOccurrences != null)
                {
                    _problemOccurrences.CollectionChanged -= ProblemOccurrences_CollectionChanged;
                }
                _problemOccurrences = value;
                if (_problemOccurrences != null)
                {
                    _problemOccurrences.CollectionChanged += ProblemOccurrences_CollectionChanged;
                }
            }
        }

        private void ProblemOccurrences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProblemOccurrence>())
                {
                    item.PriorOccurrence = this.ProblemOccurrenceId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.Agent;
            _ = this.KnowledgeRepositoryEntry;
            _ = this.ProblemOccurrence;
            _ = this.ProblemOccurrences;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
