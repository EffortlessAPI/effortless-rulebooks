
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
    [Table("ChangeValidationRuns")]
    public class ChangeValidationRunBase : SoAEntityBase
    {
        [Key]
        public string ChangeValidationRunId { get; set; }

        // Formula Name (rulebook: ={{ModelChangeRequest}} & " " & {{RunPurpose}} & " run")
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ModelChangeRequest)), F.S(" "), F.Text(F.Of(this.RunPurpose)), F.S(" run")))); set { }
        }

        public string? RunPurpose { get; set; }
        public DateTimeOffset? RanAt { get; set; }
        public int? TestCount { get; set; }
        public int? FailureCount { get; set; }
        public DateTimeOffset? FailuresInspectedAt { get; set; }
        public string? ConsistencyCheckOutcome { get; set; }
        public string? StructuralCheckOutcome { get; set; }
        public string? VocabularyCheckOutcome { get; set; }
        // Formula Release (rulebook: =INDEX(ModelChangeRequests!{{TargetRelease}}, MATCH({{ModelChangeRequest}}, ModelChangeRequests!{{ModelChangeRequestId}}, 0)))
        [NotMapped]
        public string? Release
        {
            get => F.AsString(F.Memo(this, "Release", () => F.Lookup<ModelChangeRequest>(this, "ModelChangeRequests", "ModelChangeRequestId", __c => __c.ModelChangeRequests, __r => F.Of(__r.ModelChangeRequestId), F.Of(this.ModelChangeRequest), __r => F.Of(__r.TargetRelease), () => F.Of(new ModelChangeRequest().TargetRelease)))); set { }
        }

        // Formula ExpectedChainCount (rulebook: =COUNTIFS(ExpectedInferenceChecks!{{ChangeValidationRun}}, {{ChangeValidationRunId}}))
        [NotMapped]
        public int? ExpectedChainCount
        {
            get => F.AsInt(F.Memo(this, "ExpectedChainCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ExpectedInferenceCheck>(base.SoAContext, "ExpectedInferenceChecks", __c => __c.ExpectedInferenceChecks), __r => F.CritField(F.Of(__r.ChangeValidationRun), F.Of(this.ChangeValidationRunId))))))); set { }
        }

        // Formula UnproducedChainCount (rulebook: =COUNTIFS(ExpectedInferenceChecks!{{ChangeValidationRun}}, {{ChangeValidationRunId}}, ExpectedInferenceChecks!{{IsUnproduced}}, TRUE))
        [NotMapped]
        public int? UnproducedChainCount
        {
            get => F.AsInt(F.Memo(this, "UnproducedChainCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ExpectedInferenceCheck>(base.SoAContext, "ExpectedInferenceChecks", __c => __c.ExpectedInferenceChecks), __r => F.CritField(F.Of(__r.ChangeValidationRun), F.Of(this.ChangeValidationRunId)) && F.CritLiteral(F.Of(__r.IsUnproduced), F.B(true))))))); set { }
        }

        // Formula HasUninspectedFailures (rulebook: =AND({{FailureCount}} > 0, {{FailuresInspectedAt}} = ""))
        [NotMapped]
        public bool? HasUninspectedFailures
        {
            get => F.AsBool(F.Memo(this, "HasUninspectedFailures", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.FailureCount)), ">", F.I(0))), F.Bool3(F.IsBlank(F.Of(this.FailuresInspectedAt)))))); set { }
        }

        // Formula IsUnconfirmedConsistency (rulebook: ={{ConsistencyCheckOutcome}} <> "Consistent")
        [NotMapped]
        public bool? IsUnconfirmedConsistency
        {
            get => F.AsBool(F.Memo(this, "IsUnconfirmedConsistency", () => F.Ne(F.Nullif(F.Of(this.ConsistencyCheckOutcome)), F.S("Consistent")))); set { }
        }

        // Formula MissesExpectedInference (rulebook: ={{UnproducedChainCount}} > 0)
        [NotMapped]
        public bool? MissesExpectedInference
        {
            get => F.AsBool(F.Memo(this, "MissesExpectedInference", () => F.Cmp(F.Of(this.UnproducedChainCount), ">", F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ModelChangeRequest { get; set; }
        public string? TestSuite { get; set; }

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

        private TestSuite _testSuiteRef;

        [ForeignKey("TestSuite")]
        public virtual TestSuite TestSuiteRef
        {
            get
            {
                if (_testSuiteRef == null && !string.IsNullOrEmpty(TestSuite))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TestSuiteRef - no database context is set. TestSuite: " + TestSuite + ".");
                        }
                        return null;
                    }
                    _testSuiteRef = base.SoAContext.TestSuites.Find(TestSuite);
                    if (_testSuiteRef != null)
                    {
                        base.SoAContext.Attach(_testSuiteRef);
                    }
                }
                return _testSuiteRef;
            }
            set
            {
                if (_testSuiteRef != value)
                {
                    _testSuiteRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_testSuiteRef != null)
                    {
                        TestSuite = _testSuiteRef.TestSuiteId;
                    }
                }
            }
        }

        private ObservableCollection<ExpectedInferenceCheck> _expectedInferenceChecks;

        [InverseProperty("ChangeValidationRunRef")]
        public virtual ObservableCollection<ExpectedInferenceCheck> ExpectedInferenceChecks
        {
            get
            {
                if (_expectedInferenceChecks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExpectedInferenceChecks - no database context is set. ChangeValidationRunId: " + this.ChangeValidationRunId + ".");
                        }
                        _expectedInferenceChecks = new ObservableCollection<ExpectedInferenceCheck>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExpectedInferenceChecks.Where(x => x.ChangeValidationRun == this.ChangeValidationRunId).ToList<ExpectedInferenceCheck>();
                        _expectedInferenceChecks = new ObservableCollection<ExpectedInferenceCheck>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _expectedInferenceChecks.CollectionChanged += ExpectedInferenceChecks_CollectionChanged;
                }
                return _expectedInferenceChecks;
            }
            private set
            {
                if (_expectedInferenceChecks != null)
                {
                    _expectedInferenceChecks.CollectionChanged -= ExpectedInferenceChecks_CollectionChanged;
                }
                _expectedInferenceChecks = value;
                if (_expectedInferenceChecks != null)
                {
                    _expectedInferenceChecks.CollectionChanged += ExpectedInferenceChecks_CollectionChanged;
                }
            }
        }

        private void ExpectedInferenceChecks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExpectedInferenceCheck>())
                {
                    item.ChangeValidationRun = this.ChangeValidationRunId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ModelChangeRequestRef;
            _ = this.TestSuiteRef;
            _ = this.ExpectedInferenceChecks;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
