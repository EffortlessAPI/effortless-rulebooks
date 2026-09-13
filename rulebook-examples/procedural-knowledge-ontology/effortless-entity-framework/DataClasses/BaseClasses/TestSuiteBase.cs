
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
    [Table("TestSuites")]
    public class TestSuiteBase : SoAEntityBase
    {
        [Key]
        public string TestSuiteId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        // Formula TestCount (rulebook: =COUNTIFS(TestCases!{{Suite}}, {{TestSuiteId}}))
        [NotMapped]
        public decimal? TestCount
        {
            get => F.AsDecimal(F.Memo(this, "TestCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TestCase>(base.SoAContext, "TestCases", __c => __c.TestCases), __r => F.CritField(F.Of(__r.Suite), F.Of(this.TestSuiteId)))))); set { }
        }

        // Formula PassCount (rulebook: =COUNTIFS(TestCases!{{PassingSuiteKey}}, {{TestSuiteId}}))
        [NotMapped]
        public decimal? PassCount
        {
            get => F.AsDecimal(F.Memo(this, "PassCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TestCase>(base.SoAContext, "TestCases", __c => __c.TestCases), __r => F.CritField(F.Of(__r.PassingSuiteKey), F.Of(this.TestSuiteId)))))); set { }
        }

        // Formula BlockingFailCount (rulebook: =COUNTIFS(TestCases!{{NeedsAttentionSuiteKey}}, {{TestSuiteId}}))
        [NotMapped]
        public decimal? BlockingFailCount
        {
            get => F.AsDecimal(F.Memo(this, "BlockingFailCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TestCase>(base.SoAContext, "TestCases", __c => __c.TestCases), __r => F.CritField(F.Of(__r.NeedsAttentionSuiteKey), F.Of(this.TestSuiteId)))))); set { }
        }

        // Formula IsGreen (rulebook: ={{BlockingFailCount}} = 0)
        [NotMapped]
        public bool? IsGreen
        {
            get => F.AsBool(F.Memo(this, "IsGreen", () => F.Eq(F.Of(this.BlockingFailCount), F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<TestCase> _testCases;

        [InverseProperty("TestSuite")]
        public virtual ObservableCollection<TestCase> TestCases
        {
            get
            {
                if (_testCases == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TestCases - no database context is set. TestSuiteId: " + this.TestSuiteId + ".");
                        }
                        _testCases = new ObservableCollection<TestCase>();
                    }
                    else
                    {
                        var items = base.SoAContext.TestCases.Where(x => x.Suite == this.TestSuiteId).ToList<TestCase>();
                        _testCases = new ObservableCollection<TestCase>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _testCases.CollectionChanged += TestCases_CollectionChanged;
                }
                return _testCases;
            }
            private set
            {
                if (_testCases != null)
                {
                    _testCases.CollectionChanged -= TestCases_CollectionChanged;
                }
                _testCases = value;
                if (_testCases != null)
                {
                    _testCases.CollectionChanged += TestCases_CollectionChanged;
                }
            }
        }

        private void TestCases_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TestCase>())
                {
                    item.Suite = this.TestSuiteId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.TestCases;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
