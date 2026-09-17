
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
    [Table("KnowledgeQuerySources")]
    public class KnowledgeQuerySourceBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeQuerySourceId { get; set; }

        // Formula Name (rulebook: ={{QueryDefinition}} & " reads " & {{ConsumerSystem}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.QueryDefinition)), F.S(" reads "), F.Text(F.Of(this.ConsumerSystem))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? QueryDefinition { get; set; }
        public string? ConsumerSystem { get; set; }

        private KnowledgeQueryDefinition _knowledgeQueryDefinition;

        [ForeignKey("QueryDefinition")]
        public virtual KnowledgeQueryDefinition KnowledgeQueryDefinition
        {
            get
            {
                if (_knowledgeQueryDefinition == null && !string.IsNullOrEmpty(QueryDefinition))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeQueryDefinition - no database context is set. QueryDefinition: " + QueryDefinition + ".");
                        }
                        return null;
                    }
                    _knowledgeQueryDefinition = base.SoAContext.KnowledgeQueryDefinitions.Find(QueryDefinition);
                    if (_knowledgeQueryDefinition != null)
                    {
                        base.SoAContext.Attach(_knowledgeQueryDefinition);
                    }
                }
                return _knowledgeQueryDefinition;
            }
            set
            {
                if (_knowledgeQueryDefinition != value)
                {
                    _knowledgeQueryDefinition = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeQueryDefinition != null)
                    {
                        QueryDefinition = _knowledgeQueryDefinition.KnowledgeQueryDefinitionId;
                    }
                }
            }
        }

        private KnowledgeConsumerSystem _knowledgeConsumerSystem;

        [ForeignKey("ConsumerSystem")]
        public virtual KnowledgeConsumerSystem KnowledgeConsumerSystem
        {
            get
            {
                if (_knowledgeConsumerSystem == null && !string.IsNullOrEmpty(ConsumerSystem))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeConsumerSystem - no database context is set. ConsumerSystem: " + ConsumerSystem + ".");
                        }
                        return null;
                    }
                    _knowledgeConsumerSystem = base.SoAContext.KnowledgeConsumerSystems.Find(ConsumerSystem);
                    if (_knowledgeConsumerSystem != null)
                    {
                        base.SoAContext.Attach(_knowledgeConsumerSystem);
                    }
                }
                return _knowledgeConsumerSystem;
            }
            set
            {
                if (_knowledgeConsumerSystem != value)
                {
                    _knowledgeConsumerSystem = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeConsumerSystem != null)
                    {
                        ConsumerSystem = _knowledgeConsumerSystem.KnowledgeConsumerSystemId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.KnowledgeQueryDefinition;
            _ = this.KnowledgeConsumerSystem;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
