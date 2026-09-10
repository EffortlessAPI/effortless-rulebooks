
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("TestSuites")]
    public class TestSuiteBase : SoAEntityBase
    {
        [Key]
        public string TestSuiteId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        public string? Name
        {
            get => this.Label; set { }
        }

        public string? Label { get; set; }
        // Formula TestCount (rulebook: =COUNTIFS(TestCases!{{Suite}}, {{TestSuiteId}}))
        public decimal? TestCount
        {
            get => COUNTIFS(TestCases!this.Suite, this.TestSuiteId); set { }
        }

        // Formula PassCount (rulebook: =COUNTIFS(TestCases!{{PassingSuiteKey}}, {{TestSuiteId}}))
        public decimal? PassCount
        {
            get => COUNTIFS(TestCases!this.PassingSuiteKey, this.TestSuiteId); set { }
        }

        // Formula BlockingFailCount (rulebook: =COUNTIFS(TestCases!{{NeedsAttentionSuiteKey}}, {{TestSuiteId}}))
        public decimal? BlockingFailCount
        {
            get => COUNTIFS(TestCases!this.NeedsAttentionSuiteKey, this.TestSuiteId); set { }
        }

        // Formula IsGreen (rulebook: ={{BlockingFailCount}} = 0)
        public bool? IsGreen
        {
            get => this.BlockingFailCount = 0; set { }
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TestCases - no database context is set. TestSuiteId: " + this.TestSuiteId + ".");
                        }
                        _testCases = new ObservableCollection<TestCase>();
                    }
                    else
                    {
                        var items = Context.TestCases.Where(x => x.Suite == this.TestSuiteId).ToList<TestCase>();
                        _testCases = new ObservableCollection<TestCase>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
