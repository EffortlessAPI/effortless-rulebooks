
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
    [Table("KnowledgeMethods")]
    public class KnowledgeMethodBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeMethodId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? MethodFamily { get; set; }
        public string? Summary { get; set; }
        public string? OriginReference { get; set; }
        // Formula ElicitationUseCount (rulebook: =COUNTIFS(ElicitationSessions!{{Method}}, {{KnowledgeMethodId}}))
        [NotMapped]
        public int? ElicitationUseCount
        {
            get => F.AsInt(F.Memo(this, "ElicitationUseCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ElicitationSession>(base.SoAContext, "ElicitationSessions", __c => __c.ElicitationSessions), __r => F.CritField(F.Of(__r.Method), F.Of(this.KnowledgeMethodId))))))); set { }
        }

        // Formula ApplicationCount (rulebook: =COUNTIFS(MethodApplications!{{KnowledgeMethod}}, {{KnowledgeMethodId}}))
        [NotMapped]
        public int? ApplicationCount
        {
            get => F.AsInt(F.Memo(this, "ApplicationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MethodApplication>(base.SoAContext, "MethodApplications", __c => __c.MethodApplications), __r => F.CritField(F.Of(__r.KnowledgeMethod), F.Of(this.KnowledgeMethodId))))))); set { }
        }

        // Formula UsageCount (rulebook: ={{ElicitationUseCount}} + {{ApplicationCount}})
        [NotMapped]
        public int? UsageCount
        {
            get => F.AsInt(F.Memo(this, "UsageCount", () => F.Integer(F.Add(F.Of(this.ElicitationUseCount), F.Of(this.ApplicationCount))))); set { }
        }

        // Formula IsApplied (rulebook: ={{UsageCount}} > 0)
        [NotMapped]
        public bool? IsApplied
        {
            get => F.AsBool(F.Memo(this, "IsApplied", () => F.Cmp(F.Of(this.UsageCount), ">", F.I(0)))); set { }
        }

        public string? ElicitationTradeoff { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<ElicitationSession> _elicitationSessions;

        [InverseProperty("KnowledgeMethod")]
        public virtual ObservableCollection<ElicitationSession> ElicitationSessions
        {
            get
            {
                if (_elicitationSessions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ElicitationSessions - no database context is set. KnowledgeMethodId: " + this.KnowledgeMethodId + ".");
                        }
                        _elicitationSessions = new ObservableCollection<ElicitationSession>();
                    }
                    else
                    {
                        var items = base.SoAContext.ElicitationSessions.Where(x => x.Method == this.KnowledgeMethodId).ToList<ElicitationSession>();
                        _elicitationSessions = new ObservableCollection<ElicitationSession>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _elicitationSessions.CollectionChanged += ElicitationSessions_CollectionChanged;
                }
                return _elicitationSessions;
            }
            private set
            {
                if (_elicitationSessions != null)
                {
                    _elicitationSessions.CollectionChanged -= ElicitationSessions_CollectionChanged;
                }
                _elicitationSessions = value;
                if (_elicitationSessions != null)
                {
                    _elicitationSessions.CollectionChanged += ElicitationSessions_CollectionChanged;
                }
            }
        }

        private void ElicitationSessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ElicitationSession>())
                {
                    item.Method = this.KnowledgeMethodId;
                }
            }
        }

        private ObservableCollection<ClaimEvidence> _claimEvidence;

        [InverseProperty("KnowledgeMethodRef")]
        public virtual ObservableCollection<ClaimEvidence> ClaimEvidence
        {
            get
            {
                if (_claimEvidence == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ClaimEvidence - no database context is set. KnowledgeMethodId: " + this.KnowledgeMethodId + ".");
                        }
                        _claimEvidence = new ObservableCollection<ClaimEvidence>();
                    }
                    else
                    {
                        var items = base.SoAContext.ClaimEvidence.Where(x => x.KnowledgeMethod == this.KnowledgeMethodId).ToList<ClaimEvidence>();
                        _claimEvidence = new ObservableCollection<ClaimEvidence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _claimEvidence.CollectionChanged += ClaimEvidence_CollectionChanged;
                }
                return _claimEvidence;
            }
            private set
            {
                if (_claimEvidence != null)
                {
                    _claimEvidence.CollectionChanged -= ClaimEvidence_CollectionChanged;
                }
                _claimEvidence = value;
                if (_claimEvidence != null)
                {
                    _claimEvidence.CollectionChanged += ClaimEvidence_CollectionChanged;
                }
            }
        }

        private void ClaimEvidence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ClaimEvidence>())
                {
                    item.KnowledgeMethod = this.KnowledgeMethodId;
                }
            }
        }

        private ObservableCollection<MethodApplication> _methodApplications;

        [InverseProperty("KnowledgeMethodRef")]
        public virtual ObservableCollection<MethodApplication> MethodApplications
        {
            get
            {
                if (_methodApplications == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MethodApplications - no database context is set. KnowledgeMethodId: " + this.KnowledgeMethodId + ".");
                        }
                        _methodApplications = new ObservableCollection<MethodApplication>();
                    }
                    else
                    {
                        var items = base.SoAContext.MethodApplications.Where(x => x.KnowledgeMethod == this.KnowledgeMethodId).ToList<MethodApplication>();
                        _methodApplications = new ObservableCollection<MethodApplication>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _methodApplications.CollectionChanged += MethodApplications_CollectionChanged;
                }
                return _methodApplications;
            }
            private set
            {
                if (_methodApplications != null)
                {
                    _methodApplications.CollectionChanged -= MethodApplications_CollectionChanged;
                }
                _methodApplications = value;
                if (_methodApplications != null)
                {
                    _methodApplications.CollectionChanged += MethodApplications_CollectionChanged;
                }
            }
        }

        private void MethodApplications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MethodApplication>())
                {
                    item.KnowledgeMethod = this.KnowledgeMethodId;
                }
            }
        }

        private ObservableCollection<LevelCaptureStrategy> _levelCaptureStrategies;

        [InverseProperty("KnowledgeMethodRef")]
        public virtual ObservableCollection<LevelCaptureStrategy> LevelCaptureStrategies
        {
            get
            {
                if (_levelCaptureStrategies == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LevelCaptureStrategies - no database context is set. KnowledgeMethodId: " + this.KnowledgeMethodId + ".");
                        }
                        _levelCaptureStrategies = new ObservableCollection<LevelCaptureStrategy>();
                    }
                    else
                    {
                        var items = base.SoAContext.LevelCaptureStrategies.Where(x => x.KnowledgeMethod == this.KnowledgeMethodId).ToList<LevelCaptureStrategy>();
                        _levelCaptureStrategies = new ObservableCollection<LevelCaptureStrategy>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _levelCaptureStrategies.CollectionChanged += LevelCaptureStrategies_CollectionChanged;
                }
                return _levelCaptureStrategies;
            }
            private set
            {
                if (_levelCaptureStrategies != null)
                {
                    _levelCaptureStrategies.CollectionChanged -= LevelCaptureStrategies_CollectionChanged;
                }
                _levelCaptureStrategies = value;
                if (_levelCaptureStrategies != null)
                {
                    _levelCaptureStrategies.CollectionChanged += LevelCaptureStrategies_CollectionChanged;
                }
            }
        }

        private void LevelCaptureStrategies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<LevelCaptureStrategy>())
                {
                    item.KnowledgeMethod = this.KnowledgeMethodId;
                }
            }
        }

        private ObservableCollection<KnowledgeCaptureInitiatif> _knowledgeCaptureInitiatives;

        [InverseProperty("KnowledgeMethodRef")]
        public virtual ObservableCollection<KnowledgeCaptureInitiatif> KnowledgeCaptureInitiatives
        {
            get
            {
                if (_knowledgeCaptureInitiatives == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeCaptureInitiatives - no database context is set. KnowledgeMethodId: " + this.KnowledgeMethodId + ".");
                        }
                        _knowledgeCaptureInitiatives = new ObservableCollection<KnowledgeCaptureInitiatif>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeCaptureInitiatives.Where(x => x.KnowledgeMethod == this.KnowledgeMethodId).ToList<KnowledgeCaptureInitiatif>();
                        _knowledgeCaptureInitiatives = new ObservableCollection<KnowledgeCaptureInitiatif>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeCaptureInitiatives.CollectionChanged += KnowledgeCaptureInitiatives_CollectionChanged;
                }
                return _knowledgeCaptureInitiatives;
            }
            private set
            {
                if (_knowledgeCaptureInitiatives != null)
                {
                    _knowledgeCaptureInitiatives.CollectionChanged -= KnowledgeCaptureInitiatives_CollectionChanged;
                }
                _knowledgeCaptureInitiatives = value;
                if (_knowledgeCaptureInitiatives != null)
                {
                    _knowledgeCaptureInitiatives.CollectionChanged += KnowledgeCaptureInitiatives_CollectionChanged;
                }
            }
        }

        private void KnowledgeCaptureInitiatives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeCaptureInitiatif>())
                {
                    item.KnowledgeMethod = this.KnowledgeMethodId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ElicitationSessions;
            _ = this.ClaimEvidence;
            _ = this.MethodApplications;
            _ = this.LevelCaptureStrategies;
            _ = this.KnowledgeCaptureInitiatives;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
