
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
    [Table("StewardActivities")]
    public class StewardActivityBase : SoAEntityBase
    {
        [Key]
        public string StewardActivityId { get; set; }

        // Formula Name (rulebook: ={{ModelCharter}} & " " & {{DutyKind}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ModelCharter)), F.S(" "), F.Text(F.Of(this.DutyKind))))); set { }
        }

        public string? DutyKind { get; set; }
        public DateTimeOffset? PerformedAt { get; set; }
        public string? Subject { get; set; }
        // Formula ActivityModel (rulebook: =INDEX(ModelCharters!{{GovernedModel}}, MATCH({{ModelCharter}}, ModelCharters!{{ModelCharterId}}, 0)))
        [NotMapped]
        public string? ActivityModel
        {
            get => F.AsString(F.Memo(this, "ActivityModel", () => F.Lookup<ModelCharter>(this, "ModelCharters", "ModelCharterId", __c => __c.ModelCharters, __r => F.Of(__r.ModelCharterId), F.Of(this.ModelCharter), __r => F.Of(__r.GovernedModel), () => F.Of(new ModelCharter().GovernedModel)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ModelCharter { get; set; }
        public string? PerformedByAgent { get; set; }
        public string? Release { get; set; }

        private ModelCharter _modelCharterRef;

        [ForeignKey("ModelCharter")]
        public virtual ModelCharter ModelCharterRef
        {
            get
            {
                if (_modelCharterRef == null && !string.IsNullOrEmpty(ModelCharter))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelCharterRef - no database context is set. ModelCharter: " + ModelCharter + ".");
                        }
                        return null;
                    }
                    _modelCharterRef = base.SoAContext.ModelCharters.Find(ModelCharter);
                    if (_modelCharterRef != null)
                    {
                        base.SoAContext.Attach(_modelCharterRef);
                    }
                }
                return _modelCharterRef;
            }
            set
            {
                if (_modelCharterRef != value)
                {
                    _modelCharterRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_modelCharterRef != null)
                    {
                        ModelCharter = _modelCharterRef.ModelCharterId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("PerformedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(PerformedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. PerformedByAgent: " + PerformedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(PerformedByAgent);
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
                        PerformedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private RulebookRelease _rulebookRelease;

        [ForeignKey("Release")]
        public virtual RulebookRelease RulebookRelease
        {
            get
            {
                if (_rulebookRelease == null && !string.IsNullOrEmpty(Release))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookRelease - no database context is set. Release: " + Release + ".");
                        }
                        return null;
                    }
                    _rulebookRelease = base.SoAContext.RulebookReleases.Find(Release);
                    if (_rulebookRelease != null)
                    {
                        base.SoAContext.Attach(_rulebookRelease);
                    }
                }
                return _rulebookRelease;
            }
            set
            {
                if (_rulebookRelease != value)
                {
                    _rulebookRelease = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookRelease != null)
                    {
                        Release = _rulebookRelease.RulebookReleaseId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ModelCharterRef;
            _ = this.Agent;
            _ = this.RulebookRelease;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
